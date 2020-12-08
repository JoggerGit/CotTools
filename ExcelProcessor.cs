﻿using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace CotTools
{
    /// <summary>
    /// ExcelProcessor
    /// </summary>
    public class ExcelProcessor
    {
        /// <summary>
        /// Financials
        /// </summary>
        public partial class Financials : ConverterBase
        {
            /// <summary>
            /// ProcessForexNet
            /// </summary>
            /// <param name="fileWithPath"></param>
            /// <param name="dateColumnIndex"></param>
            /// <param name="columnIndex"></param>
            /// <param name="invertResults"></param>
            /// <returns></returns>
            public static string ProcessForexNet(string fileWithPath, int dateColumnIndex, int columnIndex, bool invertResults)
            {
                ForexWorkbook forexWorkbook = new ForexWorkbook(fileWithPath);
                StringBuilder stringBuilder = new StringBuilder();
                const string SEPARATOR = ";";

                // Validate
                string message;
                if ((message = forexWorkbook.Validate()) != string.Empty)
                {
                    return message;
                    //MessageBox.Show(message);
                }

                // For each line in Excel
                Cells cellsEur = forexWorkbook.WorksheetEur.Cells;
                int rowCountEur = cellsEur.MaxDataRow;
                for (int row = 1; row <= rowCountEur; row++)
                {
                    // Data from different worksheets in dictionary
                    Dictionary<string, int> dictCurrencyValue = new Dictionary<string, int>();
                    dictCurrencyValue[Forex.EUR] = cellsEur[row, columnIndex].Value.ToNetValue();
                    dictCurrencyValue[Forex.AUD] = forexWorkbook.WorksheetAud.Cells[row, columnIndex].Value.ToNetValue();
                    dictCurrencyValue[Forex.CAD] = forexWorkbook.WorksheetCad.Cells[row, columnIndex].Value.ToNetValue();
                    dictCurrencyValue[Forex.CHF] = forexWorkbook.WorksheetChf.Cells[row, columnIndex].Value.ToNetValue();
                    dictCurrencyValue[Forex.GBP] = forexWorkbook.WorksheetGbp.Cells[row, columnIndex].Value.ToNetValue();
                    dictCurrencyValue[Forex.JPY] = forexWorkbook.WorksheetJpy.Cells[row, columnIndex].Value.ToNetValue();
                    dictCurrencyValue[Forex.USD] = (int)Math.Round((double)(dictCurrencyValue[Forex.EUR] + dictCurrencyValue[Forex.AUD] + dictCurrencyValue[Forex.CAD] + dictCurrencyValue[Forex.CHF]
                        + dictCurrencyValue[Forex.GBP] + dictCurrencyValue[Forex.JPY]) / -6);


                    // Currencies mit max/min values
                    var max = invertResults
                       ? GetCurrencyOfMaxMinValue(dictCurrencyValue, int.MaxValue, (a, b) => a < b)
                       : GetCurrencyOfMaxMinValue(dictCurrencyValue, int.MinValue, (a, b) => a > b);
                    var min = invertResults
                       ? GetCurrencyOfMaxMinValue(dictCurrencyValue, int.MinValue, (a, b) => a > b)
                       : GetCurrencyOfMaxMinValue(dictCurrencyValue, int.MaxValue, (a, b) => a < b);
                    var bestPairUnnormilized = max.Key + min.Key;
                    KeyValuePair<string, int> bestPairWithDirection = NormalizeCurrencyPair(bestPairUnnormilized);

                    // Get date
                    DateTime date = GetDate(cellsEur[row, dateColumnIndex]);
                    if (date == DateTime.MinValue)
                    {
                        message = $"DateTime can not be parsed from string {cellsEur[row, dateColumnIndex].Value.ToString()}";
                        //MessageBox.Show();
                        return message;
                    }

                    // Fill string builder
                    stringBuilder.Append($"{date.ToString("dd.MM.yyyy")}{SEPARATOR}{bestPairWithDirection.Key}{SEPARATOR}{bestPairWithDirection.Value}{Environment.NewLine}");
                }

                return stringBuilder.ToString();
            }

            internal static void ProcessDealerInverted(string fileName)
            {
                throw new NotImplementedException();
            }

            public static void Example(string fileWithPath)
            {
                //Open your template file.
                Workbook wb = new Workbook(fileWithPath);

                //Get the first worksheet.
                Worksheet worksheet = wb.Worksheets[0];

                //Get cells
                Cells cells = worksheet.Cells;

                // Get row and column count
                int rowCount = cells.MaxDataRow;
                int columnCount = cells.MaxDataColumn;

                // Current cell value
                string strCell = "";

                Console.WriteLine(String.Format("rowCount={0}, columnCount={1}", rowCount, columnCount));

                for (int row = 0; row <= rowCount; row++) // Numeration starts from 0 to MaxDataRow
                {
                    for (int column = 0; column <= columnCount; column++)  // Numeration starts from 0 to MaxDataColumn
                    {
                        strCell = "";
                        strCell = Convert.ToString(cells[row, column].Value);
                        if (String.IsNullOrEmpty(strCell))
                        {
                            continue;
                        }
                        else
                        {
                            // Do your staff here
                            Console.WriteLine(strCell);
                        }
                    }
                }
            }

        }
    }
}
