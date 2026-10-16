# Installation des outils Claude Code du projet (à lancer par l'utilisateur, une fois).
#   powershell -ExecutionPolicy Bypass -File scripts\installer-outils.ps1
# Chaque étape demande confirmation. Rien n'est installé sans votre accord.

$ErrorActionPreference = 'Stop'
$Projet = Split-Path -Parent $PSScriptRoot
Set-Location $Projet

function Confirmer($Question) {
    $r = Read-Host "$Question [o/N]"
    return $r -match '^(o|oui|y|yes)$'
}
function Present($Commande) { return [bool](Get-Command $Commande -ErrorAction SilentlyContinue) }

Write-Host "`n== Outils Claude Code — GestionProjet ==" -ForegroundColor Cyan

# 0. openpyxl (export Excel du backlog)
python -c "import openpyxl" 2>$null
if ($LASTEXITCODE -ne 0) {
    if (Confirmer "Installer le module Python openpyxl (vue Excel du backlog) ?") { python -m pip install openpyxl }
} else { Write-Host "openpyxl : présent" -ForegroundColor Green }

# 1. Node.js LTS (hooks Ponytail)
if (-not (Present 'node')) {
    if (Confirmer "Node.js est absent (requis pour l'activation automatique de Ponytail). Installer Node.js LTS via winget ?") {
        winget install --id OpenJS.NodeJS.LTS -e --accept-package-agreements
        Write-Host "Ouvrez un nouveau terminal après l'installation pour que 'node' soit dans le PATH." -ForegroundColor Yellow
    }
} else { Write-Host "Node.js : $(node --version)" -ForegroundColor Green }

# 2. uv + graphify (graphe de connaissances du code)
if (-not (Present 'uv')) {
    if (Confirmer "uv est absent (gestionnaire d'outils Python, requis pour graphify). Installer uv via winget ?") {
        winget install --id astral-sh.uv -e --accept-package-agreements
        $env:Path += ";$env:USERPROFILE\.local\bin"
    }
}
if (Present 'uv') {
    if (-not (Present 'graphify')) {
        if (Confirmer "Installer graphify (paquet PyPI 'graphifyy') ?") { uv tool install graphifyy; uv tool update-shell }
    }
    if ((Present 'graphify') -and (Confirmer "Enregistrer graphify dans ce projet (graphify install --project : skill + hooks dans .claude/) ?")) {
        graphify install --project
        Write-Host "graphify ajoute ses hooks à .claude/settings.json : vérifiez le fichier avec git diff." -ForegroundColor Yellow
    }
}

# 3. Skill motion-design (LottieFiles) copié dans .claude/skills
$Motion = Join-Path $Projet '.claude\skills\motion-design'
if (-not (Test-Path $Motion)) {
    if (Confirmer "Copier le skill motion-design (github.com/LottieFiles/motion-design-skill) dans .claude/skills ?") {
        $Tmp = Join-Path $env:TEMP "motion-design-skill-$(Get-Random)"
        git clone --depth 1 https://github.com/LottieFiles/motion-design-skill.git $Tmp
        Copy-Item -Recurse (Join-Path $Tmp 'skills\motion-design') $Motion
        Remove-Item -Recurse -Force $Tmp
        Write-Host "motion-design installé : $Motion" -ForegroundColor Green
    }
} else { Write-Host "motion-design : déjà présent" -ForegroundColor Green }

# 4. Docker (requis à partir de la phase P3, hypothèse H-001)
if (-not (Present 'docker')) {
    if (Confirmer "Docker est absent (requis en P3). Installer Docker Desktop via winget (droits administrateur et WSL 2 nécessaires) ?") {
        winget install --id Docker.DockerDesktop -e --accept-package-agreements
    }
} else { Write-Host "Docker : $(docker --version)" -ForegroundColor Green }

Write-Host "`n== Plugins (dans Claude Code) ==" -ForegroundColor Cyan
Write-Host @"
Ponytail et UI/UX Pro Max sont déclarés dans .claude/settings.json (extraKnownMarketplaces + enabledPlugins).
Au premier lancement de 'claude' dans ce dossier, acceptez la confiance du dossier puis l'installation proposée.
Sinon, tapez dans Claude Code :
  /plugin marketplace add DietrichGebert/ponytail
  /plugin install ponytail@ponytail
  /plugin marketplace add nextlevelbuilder/ui-ux-pro-max-skill
  /plugin install ui-ux-pro-max@ui-ux-pro-max-skill
Vérification : python scripts\verifier-env.py (créé en P0-T02) et /etat-projet.
"@
