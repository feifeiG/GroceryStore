using Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExcelParser : EditorWindow
{
    [MenuItem("Custom/ExcelParser")]
    private static void CreateExcel()
    {
        string path = Application.dataPath + "/Editor/Excel/";
        string[] files = Directory.GetFiles(path, "*.xlsx"); 
        foreach(string file in files)
        {
            Parser(file);
        }
    }

    private static void Parser(string path)
    {
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

        DataSet result = excelReader.AsDataSet();
        DataTable describe = result.Tables[0];
        if (describe == null) {
            return;
        }

        Dictionary<string, string> file_map = new Dictionary<string, string>();
        Dictionary<string, string> key_map = new Dictionary<string, string>();
        for(int i = 1; i < describe.Rows.Count; i++)
        {
            string table = describe.Rows[i][0] as string;
            string file = describe.Rows[i][1] as string;
            string key = describe.Rows[i][2] as string;

            file_map[table] = file;
            key_map[table] = key;
        }

        for (int i = 1; i < result.Tables.Count; i++) {
            Dictionary<object, Dictionary<string, object>> config = new Dictionary<object, Dictionary<string, object>>();
            DataTable table = result.Tables[i];

            int key_idx = -1;
            Dictionary<int, string> field_map = new Dictionary<int, string>();
            Dictionary<int, string> type_map = new Dictionary<int, string>();
            for (int col = 0; col < table.Columns.Count; col++)
            {
                if (table.Rows[0][col].ToString().Equals(key_map[table.TableName]))
                {
                    key_idx = col;
                }

                field_map[col] = table.Rows[0][col].ToString();
                type_map[col] = table.Rows[1][col].ToString();
            }

            for (int row = 3; row < table.Rows.Count; row++)
            {
                Dictionary<string, object> part = new Dictionary<string, object>();
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    string key = field_map[col];
                    object value = ParserType(type_map[col], table.Rows[row][col]);
                    part[key] = value;
                }
                config[table.Rows[row][key_idx] ?? config.Count] = part;
            }

            if (file_map[table.TableName] != null) {
                SaveAsJson(file_map[table.TableName], config);
            }
        }
    }

    private static object ParserType(string type, object content)
    {
        object obj;
        switch (type)
        {
            case "int": {
                    obj = Convert.ToInt32(content);
                }
                break;
            case "string": {
                    obj = content.ToString();
                }
                break;
            case "array1-1": {
                    obj = content.ToString();
                }
                break;
            case "array2-1": {
                    obj = content.ToString();
                }
                break;
            default:
                {
                    obj = (string)content;
                }
                break;
        }
        return obj;
    }

    private static void SaveAsJson(string file_name, object config)
    {
        string json = JsonConvert.SerializeObject(config);
        string path = Application.dataPath + "/Editor/Excel/Json/" + file_name + ".json";

        FileStream fs = File.Create(path);
        byte[] bts = System.Text.Encoding.UTF8.GetBytes(json);
        fs.Write(bts, 0, bts.Length);
        fs.Close();
    }
}
