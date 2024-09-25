//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System;
//using Excel = Microsoft.Office.Interop.Excel;

//namespace L09_MessageConstruction

//{
//    public class ExcelAdapter
//    {
//        private Excel.Application _excelApp;
//        private Excel._Workbook _workbook;
//        private Excel._Worksheet _worksheet;

//        public ExcelAdapter()
//        {
//            _excelApp = new Excel.Application();
//            _excelApp.Visible = true;
//            _workbook = (Excel._Workbook)(_excelApp.Workbooks.Add());
//            _worksheet = (Excel._Worksheet)_workbook.ActiveSheet;

//            // Initialiser kolonneoverskrifter
//            _worksheet.Cells[1, 1] = "Flight Number";
//            _worksheet.Cells[1, 2] = "Estimated Time of Arrival";
//            _worksheet.get_Range("A1", "B1").Font.Bold = true;
//        }

//        public void AddFlightInfo(string flightNumber, string eta)
//        {
//            int row = _worksheet.UsedRange.Rows.Count + 1;

//            _worksheet.Cells[row, 1] = flightNumber;
//            _worksheet.Cells[row, 2] = eta;

//            // AutoFit kolonner
//            _worksheet.Columns.AutoFit();
//        }

//        public void Close()
//        {
//            _workbook.SaveAs(@"C:\Path\To\Your\File.xlsx"); // Skift stien til din ønskede placering
//            _workbook.Close();
//            _excelApp.Quit();
//        }
//    }
//}
