using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

//[InitializeOnLoad]
public class ExcelParser {
    static ExcelParser()
    {
        string path = Application.dataPath + "/Editor/Excel/";
        string[] files = Directory.GetFiles(path, "*.xlsx"); 
        foreach(string file in files)
        {
            Parser(file);
        }
    }

    public static void Parser(string file)
    {
        FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        DataSet result = excelReader.AsDataSet();
    
            Debug.Log(result.Tables[0].TableName);
        //int columns = result.Tables[0].Columns.Count;
        //int rows = result.Tables[0].Rows.Count;

        //for (int i = 0; i < rows; i++)
        //{
        //    for (int j = 0; j < columns; j++)
        //    {
        //        string nvalue = result.Tables[0].Rows[i][j].ToString();
        //        Debug.Log(nvalue);
        //    }
        //}
    }
}
