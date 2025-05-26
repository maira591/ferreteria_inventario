using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ferreteria.DataAccess.Utils
{
    public static class Util
    {
        private static char GetHexValue(int i)
        {
            return i < 10 ? (char)(i + 48) : (char)(i - 10 + 65);
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


        private static void ChangeType(this DataColumn column, Type type, Func<object, object> parser)
        {
            int ordinal = column.Ordinal;
            //si no es tabla solo cambia el tipo de dato
            if (column.Table == null)
            {
                column.DataType = type;
                return;
            }

            //Clona la tabla
            DataTable clonedtable = column.Table.Clone();

            //Obtiene la columna a clonar
            DataColumn clonedcolumn = clonedtable.Columns[column.ColumnName];

            //Remueve la columna clonada de la tabla clonada
            clonedtable.Columns.Remove(clonedcolumn);

            //cambia el tipo de dato en la columna clonada
            clonedcolumn.DataType = type;

            //asigna un nombre temporal a la columna clonada
            clonedcolumn.ColumnName = Guid.NewGuid().ToString();

            //adiciona la columna clonada a la tabla
            column.Table.Columns.Add(clonedcolumn);

            //llena las filas
            foreach (DataRow drRow in column.Table.Rows)
            {
                drRow[clonedcolumn] = parser(drRow[column]);
            }

            //remueve la columna original de la tabla
            column.Table.Columns.Remove(column);

            //asigna el nombre y la posicion de la columna original a la clonada
            clonedcolumn.ColumnName = column.ColumnName;
            clonedcolumn.SetOrdinal(ordinal);
        }

        public static void ChangeColumnTypesId(DataTable table)
        {
            List<DataColumn> lsColumns = table.Columns
            .Cast<DataColumn>()
            .Where(i => i.DataType == typeof(byte[]))
            .ToList();

            //recorre las columnas de tipo byte
            foreach (DataColumn column in lsColumns)
            {
                //cambia su valor a string
                column.ChangeType(typeof(string), (value) =>
                {
                    return ToOracleString(new Guid((byte[])value));
                });
            }
        }
    }
}
