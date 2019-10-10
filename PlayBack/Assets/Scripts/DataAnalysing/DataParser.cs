using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataParser : MonoBehaviour
{

    public enum UserNames { Alex, Ali, Alireza, Homayoun, Janik, Maren, Mohammad, Nora };
    public enum Methods { Jump, Lerp, OnNotVisible, SLAM };
    public enum Values { Time, Position, Orientation, Adjustment };

    public UserNames user;
    private int userNumber;

    public Values value;
    private int valueNumber;

    private string[][] scenarios;
    private string[][] printableResults;
    private string[][] allUsersData;
    private string[][] reportData;

    public TMPro.TMP_Dropdown userDropDown;
    public TMPro.TMP_Dropdown valueDropDown;
    public TMPro.TMP_Text[] textsInTable;

    void Start()
    {
        scenarios = new string[32][];
        for (int i = 0; i < 32; i++)
        {
            scenarios[i] = new string[25];
        }

        printableResults = new string[4][];
        for (int i = 0; i < 4; i++)
        {
            printableResults[i] = new string[4];
        }

        allUsersData = new string[4][];
        for (int i = 0; i < 4; i++)
        {
            allUsersData[i] = new string[32];
        }

        reportData = new string[4][];
        for (int i = 0; i < 4; i++)
        {
            reportData[i] = new string[8];
        }

        ReadFromFile();
    }

    void Update()
    {
        //print(user.ToString() + "--->" + userNumber);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //PopulateArraysVertically(valueNumber + 6);
            //PopulateArraysHorizontally(valueNumber + 6);
            PopulateArrayWithSingleValues(3);
        }
    }


    int index0 = 0;
    int index1 = 0;
    int index2 = 0;
    int index3 = 0;

    void PopulateArrayWithSingleValues(int firstIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int u = 0; u < 8; u++)
            {
                if (scenarios[(u * 4) + i][2] == Methods.Jump.ToString())
                {
                    reportData[0][index0] = scenarios[(u * 4) + i][firstIndex];
                    index0++;
                }
                else if (scenarios[(u * 4) + i][2] == Methods.Lerp.ToString())
                {
                    reportData[1][index1] = scenarios[(u * 4) + i][firstIndex];
                    index1++;
                }
                else if (scenarios[(u * 4) + i][2] == Methods.OnNotVisible.ToString())
                {
                    reportData[2][index2] = scenarios[(u * 4) + i][firstIndex];
                    index2++;
                }
                else
                {
                    reportData[3][index3] = scenarios[(u * 4) + i][firstIndex];
                    index3++;
                }
            }
        }

        Print2dArray(reportData);
    }

    void PopulateArraysHorizontally(int firstIndex)
    {
        for (int scenario = 0; scenario < 32; scenario++)
        {
            for (int row = 0; row < 4; row++)
            {
                for (int method = 0; method < 4; method++)
                {
                    allUsersData[row][scenario] = scenarios[scenario][firstIndex + (row * 4)];
                }
            }
        }

        Print2dArray(allUsersData);
    }

    void PopulateArraysVertically(int firstIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int u = 0; u < 8; u++)
            {
                if (scenarios[(u * 4) + i][2] == Methods.Jump.ToString())
                {
                    for (int j = 0; j < 4; j++)
                    {
                        allUsersData[0][u * 4 + j] = scenarios[(u * 4) + i][firstIndex + (j * 4)];
                    }
                }
                else if (scenarios[(u * 4) + i][2] == Methods.Lerp.ToString())
                {
                    for (int j = 0; j < 4; j++)
                    {
                        allUsersData[1][u * 4 + j] = scenarios[(u * 4) + i][firstIndex + (j * 4)];
                    }
                }
                else if (scenarios[(u * 4) + i][2] == Methods.OnNotVisible.ToString())
                {
                    for (int j = 0; j < 4; j++)
                    {
                        allUsersData[2][u * 4 + j] = scenarios[(u * 4) + i][firstIndex + (j * 4)];
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        allUsersData[3][u * 4 + j] = scenarios[(u * 4) + i][firstIndex + (j * 4)];
                    }
                }
            }

        }

        Print2dArray(allUsersData);
    }


    public void UpdatePrintableResults()
    {
        PopulateTableWithData(valueNumber + 6);
    }

    void PopulateTableWithData(int firstIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            if (scenarios[(userNumber * 4) + i][2] == Methods.Jump.ToString())
            {
                printableResults[0][0] = scenarios[(userNumber * 4) + i][firstIndex];
                printableResults[1][0] = scenarios[(userNumber * 4) + i][firstIndex + 4];
                printableResults[2][0] = scenarios[(userNumber * 4) + i][firstIndex + 8];
                printableResults[3][0] = scenarios[(userNumber * 4) + i][firstIndex + 12];
            }
            else if (scenarios[(userNumber * 4) + i][2] == Methods.Lerp.ToString())
            {
                printableResults[0][1] = scenarios[(userNumber * 4) + i][firstIndex];
                printableResults[1][1] = scenarios[(userNumber * 4) + i][firstIndex + 4];
                printableResults[2][1] = scenarios[(userNumber * 4) + i][firstIndex + 8];
                printableResults[3][1] = scenarios[(userNumber * 4) + i][firstIndex + 12];
            }
            else if (scenarios[(userNumber * 4) + i][2] == Methods.OnNotVisible.ToString())
            {
                printableResults[0][2] = scenarios[(userNumber * 4) + i][firstIndex];
                printableResults[1][2] = scenarios[(userNumber * 4) + i][firstIndex + 4];
                printableResults[2][2] = scenarios[(userNumber * 4) + i][firstIndex + 8];
                printableResults[3][2] = scenarios[(userNumber * 4) + i][firstIndex + 12];
            }
            else
            {
                printableResults[0][3] = scenarios[(userNumber * 4) + i][firstIndex];
                printableResults[1][3] = scenarios[(userNumber * 4) + i][firstIndex + 4];
                printableResults[2][3] = scenarios[(userNumber * 4) + i][firstIndex + 8];
                printableResults[3][3] = scenarios[(userNumber * 4) + i][firstIndex + 12];
            }
        }
        UpdateTable();
        //Print2dArray(printableResults);
    }

    void UpdateTable()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                textsInTable[i * 4 + j].text = printableResults[i][j];
            }
        }
    }

    void PrintArray(string[] array)
    {
        string result = "";
        for (int i = 0; i < array.Length; i++)
        {
            result += array[i] + ",";
        }
        print(result);
    }

    void Print2dArray(string[][] array)
    {
        string result = "";
        for (int row = 0; row < array.Length; row++)
        {
            for (int column = 0; column < array[0].Length; column++)
            {
                //result += array[row][column] + "\t \t";
                result += array[row][column] + ",";
            }
            result += "\n";
        }
        print(result);
    }

    public void UpdateValueNumber()
    {
        switch (valueDropDown.value)
        {
            case 0:
                value = Values.Time;
                valueNumber = 0;
                break;
            case 1:
                value = Values.Position;
                valueNumber = 1;
                break;
            case 2:
                value = Values.Orientation;
                valueNumber = 2;
                break;
            case 3:
                value = Values.Adjustment;
                valueNumber = 3;
                break;
        }
    }

    public void UpdateUserNumber()
    {
        switch (userDropDown.value)
        {
            case 0:
                user = UserNames.Alex;
                userNumber = 0;
                break;
            case 1:
                user = UserNames.Ali;
                userNumber = 1;
                break;
            case 2:
                user = UserNames.Alireza;
                userNumber = 2;
                break;
            case 3:
                user = UserNames.Homayoun;
                userNumber = 3;
                break;
            case 4:
                user = UserNames.Janik;
                userNumber = 4;
                break;
            case 5:
                user = UserNames.Maren;
                userNumber = 5;
                break;
            case 6:
                user = UserNames.Mohammad;
                userNumber = 6;
                break;
            case 7:
                user = UserNames.Nora;
                userNumber = 7;
                break;
        }
        //switch (user)
        //{
        //    case UserNames.Alex:
        //        userNumber = 0;
        //        break;
        //    case UserNames.Ali:
        //        userNumber = 1;
        //        break;
        //    case UserNames.Alireza:
        //        userNumber = 2;
        //        break;
        //    case UserNames.Homayoun:
        //        userNumber = 3;
        //        break;
        //    case UserNames.Janik:
        //        userNumber = 4;
        //        break;
        //    case UserNames.Maren:
        //        userNumber = 5;
        //        break;
        //    case UserNames.Mohammad:
        //        userNumber = 6;
        //        break;
        //    default:
        //        userNumber = 7;
        //        break;
        //}

    }

    void ReadFromFile()
    {
        string path = Application.dataPath + "/Data/FinalData/FinalData.csv";
        string data = File.ReadAllText(path);
        string[] csv = data.Split(',');
        print(csv.Length);
        for (int scenarioIndex = 0; scenarioIndex < 32; scenarioIndex++)
        {
            for (int dataPoint = 0; dataPoint < 25; dataPoint++)
            {
                scenarios[scenarioIndex][dataPoint] = csv[25 * scenarioIndex + dataPoint];
            }
        }
    }

    string GetReports()
    {
        string reports = "";
        for (int i = 0; i < 32; i++)
        {
            reports += scenarios[i][5] + ",";
        }
        return reports;
    }

}
