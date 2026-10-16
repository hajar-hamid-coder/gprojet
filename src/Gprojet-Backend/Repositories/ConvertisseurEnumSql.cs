using Gprojet.Backend.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Gprojet.Backend.Repositories;

/// <summary>Énumération C# ⇄ texte des <c>CHECK (… IN (…))</c> SQL, via <see cref="ValeurEnumSql"/>.</summary>
public sealed class ConvertisseurEnumSql<TEnum>() : ValueConverter<TEnum, string>(
    v => ValeurEnumSql.VersSql(v),
    s => ValeurEnumSql.DepuisSql<TEnum>(s))
    where TEnum : struct, Enum;
