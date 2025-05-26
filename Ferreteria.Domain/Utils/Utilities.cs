using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ferreteria.Domain.Utils
{
    public static class Utilities
    {

        public static string SetSchema(string query, string schema)
        {
            return query.Replace("[schema]", schema);
        }
        public static bool DecimalParse(string value, string decimalSeparator, out decimal decimalvalue)
        {
            decimalvalue = -1;
            try
            {
                value = value.Replace(",", decimalSeparator);
                value = value.Replace(".", decimalSeparator);
                decimalvalue = decimal.Parse(value, new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = decimalSeparator });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static DateTime? DateParse(string date, string format = "dd/MM/yyyy")
        {
            try
            {
                return DateTime.ParseExact(date, format, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static bool ValidateLenghtDecimal(decimal valorCasteado, double longitudParametrizada)
        {
            long longitudParteEnteraParametrizado = GetNumberPart(longitudParametrizada.ToString(), NumberPart.Integer.GetHashCode());
            long longitudParteDecimalParametrizado = GetNumberPart(longitudParametrizada.ToString(), NumberPart.Decimal.GetHashCode());
            longitudParteEnteraParametrizado = longitudParteEnteraParametrizado - longitudParteDecimalParametrizado;

            long longitudParteEnteraValorCasteado = GetNumberPart(valorCasteado.ToString(), NumberPart.Integer.GetHashCode());
            long longitudParteDecimalValorCasteado = GetNumberPart(valorCasteado.ToString(), NumberPart.Decimal.GetHashCode());


            return (longitudParteEnteraParametrizado < longitudParteEnteraValorCasteado.ToString().Length ||
                 (longitudParteDecimalValorCasteado == 0 ?
                 longitudParteDecimalParametrizado < longitudParteDecimalValorCasteado :
                 longitudParteDecimalParametrizado < longitudParteDecimalValorCasteado.ToString().Length)
                    );
        }

        public static long GetNumberPart(string valor, int part)
        {
            if (!string.IsNullOrWhiteSpace(valor) && (valor.IndexOf(".") >= 0 || valor.IndexOf(",") >= 0))
            {
                valor = valor.Replace(",", ".");
                return long.Parse(valor.Split('.')[part]);
            }
            else if (part == 0)
            {
                return long.Parse(valor);
            }

            return 0;
        }

        public static bool ValidateSQLSentenceReservedWords(string sqlQuery, List<string> reservedWords)
        {
            sqlQuery = sqlQuery.Replace("\"", "").ToUpper();

            foreach (string reservedWord in reservedWords)
            {
                var regex = new Regex(@"(?<![\w])" + reservedWord + @"(?![\w])", RegexOptions.IgnoreCase);
                var match = Regex.Match(sqlQuery, regex.ToString());

                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }

        public static string ToOracleString(Guid value)
        {
            var bytes = value.ToByteArray();
            int len = bytes.Length * 2;
            char[] chars = new char[len];
            int bi = 0;
            int ci = 0;
            while (ci < len)
            {
                byte b = bytes[bi++];
                chars[ci] = GetHexValue(b / 16);
                chars[ci + 1] = GetHexValue(b % 16);
                ci += 2;
            }

            return new string(chars, 0, chars.Length);
        }

        public static string RemoveAccentsAndQuotes(string nameFile)
        {
            string cleanText = Regex.Replace(nameFile.ToUpper(), @"[^A-Za-z0-9]", string.Empty);
            cleanText = cleanText.Replace("(", "").Replace(")", "").Replace("-", "");
            string normalizedString = cleanText.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            cleanText = sb.ToString();

            return cleanText;
        }

        public static string RemoveCurrency(string input)
        {
            input = input.Replace("&", "").Replace("#", "")
                    .Replace("%", "").Replace("€", "").Replace("/", "").Replace(@"\", "").Replace("*", "").Replace("+", "").Replace("_", "");

            if (input.Contains("$"))
            {
                input = input.Replace("$", "").Replace(".", "");
            }

            if (input.Trim() == "-")
            {
                input = string.Empty;
            }


            return input.Trim();
        }

        public static string FormatDate(DateTime fecha)
        {
            CultureInfo cultura = new CultureInfo("es-ES");
            string fechaFormateada = $"{ObtenerNumeroEnLetras(fecha.Day)} ({fecha.Day}) de {fecha.ToString("MMMM", cultura)} de {ObtenerAnioEnLetras(fecha.Year)} ({fecha.Year})";

            return fechaFormateada;
        }


        #region Private methods
        private static char GetHexValue(int i)
        {
            return i < 10 ? (char)(i + 48) : (char)(i - 10 + 65);
        }



        static string ObtenerNumeroEnLetras(int numero)
        {
            string[] unidades = { "", "Uno", "Dos", "Tres", "Cuatro", "Cinco", "Seis", "Siete", "Ocho", "Nueve" };
            string[] unidadesDecenas = { "", "Once", "Doce", "Trece", "Catorce", "Quince" };
            string[] decenas = { "", "Diez", "Veinte", "Treinta", "Cuarenta", "Cincuenta", "Sesenta", "Setenta", "Ochenta", "Noventa" };

            if (numero == 0)
            {
                return "";
            }
            else if (numero < 10)
            {
                return unidades[numero].ToLower();
            }
            else if (numero < 20)
            {
                if (numero - 10 <= 5)
                {
                    return unidadesDecenas[numero - 10].ToLower();
                }
                return $"Dieci{unidades[numero - 10]}".ToLower();
            }
            else
            {
                int unidad = numero % 10;
                int decena = numero / 10;

                if (unidad == 0)
                {
                    return decenas[decena];
                }
                else
                {
                    if (decenas[decena].ToLower() == "veinte")
                    {
                        return $"Veinti{unidades[unidad]}".ToLower();
                    }
                    return $"{decenas[decena]} y {unidades[unidad]}".ToLower();
                }
            }
        }

        static string ObtenerAnioEnLetras(int anio)
        {
            string[] miles = { "", "Mil", "Dos Mil" };
            string[] centenas = { "", "Ciento", "Doscientos", "Trescientos", "Cuatrocientos", "Quinientos", "Seiscientos", "Setecientos", "Ochocientos", "Novecientos" };

            if (anio >= 1 && anio <= 999)
            {
                int centena = anio / 100;
                int resto = anio % 100;

                return $"{centenas[centena]} {ObtenerNumeroEnLetras(resto)}".ToLower();
            }
            else if (anio >= 1000 && anio <= 9999)
            {
                int mil = anio / 1000;
                int centena = (anio % 1000) / 100;
                int resto = anio % 100;

                return $"{miles[mil]} {centenas[centena]} {ObtenerNumeroEnLetras(resto)}".ToLower().Replace("  ", " ");
            }
            else
            {
                return "Año fuera de rango";
            }
        }


        #endregion
    }

    public enum MonthsEnum
    {
        Mensual = 1,
        Bimensual = 2,
        Trimestral = 3,
        Semestral = 6,
        Anual = 12
    }
}
