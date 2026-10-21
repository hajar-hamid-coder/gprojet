#!/usr/bin/env python3
"""P2-T02 : app.css du front = app.css de la charte, a une seule difference pres.

Definition de "seule la ligne @import differe" : la ligne `@import url(...)` de la charte est
remplacee par un bloc contigu ne contenant que des regles @font-face. Tout ce qui precede et
tout ce qui suit le bloc est identique, octet pour octet (lignes).
"""
import re
import sys
from pathlib import Path

racine = Path(__file__).resolve().parent.parent
charte = (racine / "design/charte-hcp/app.css").read_text(encoding="utf-8").split("\n")
front = (racine / "src/Gprojet-Front/wwwroot/css/app.css").read_text(encoding="utf-8").split("\n")

imports = [i for i, l in enumerate(charte) if l.startswith("@import")]
if len(imports) != 1:
    sys.exit(f"KO : {len(imports)} ligne(s) @import dans la charte (1 attendue)")
i = imports[0]
queue = len(charte) - i - 1
if front[:i] != charte[:i] or front[len(front) - queue:] != charte[i + 1:]:
    sys.exit("KO : app.css differe de la charte ailleurs que sur la ligne @import")
bloc = "\n".join(front[i:len(front) - queue])
if not bloc.strip() or "@import" in bloc or "http" in bloc \
        or re.sub(r"@font-face\s*\{[^{}]*\}", "", bloc).strip():
    sys.exit("KO : le remplacement de @import n'est pas un bloc de @font-face locaux")
print(f"OK : seule la ligne {i + 1} (@import) est remplacee par {bloc.count('@font-face')} @font-face locaux")
