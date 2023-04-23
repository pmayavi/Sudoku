using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class SudokuScriptOLD : MonoBehaviour
{
    private List<GameObject> grid = new List<GameObject>();
    private int sy;
    private int sx;
    private int syx;
    private int[,] sudoku;
    private int[,] mat;
    private List<int[]> process;
    private bool solved;
    public Button buttonSolveFast;
    public Button buttonSolveSlow;
    public Button buttonRandomize;
    public Button buttonRemove;
    public Button returns;
    public InputField input;
    public GameObject select;
    public float time;
    public string filedir;

    void Start()
    {
        input.onEndEdit.AddListener(delegate { LockInput(); });
        buttonSolveFast.onClick.AddListener(ButtonSolveFast);
        buttonSolveSlow.onClick.AddListener(ButtonSolveSlow);
        buttonRandomize.onClick.AddListener(ButtonRandomize);
        buttonRemove.onClick.AddListener(removeNumbers);
        returns.onClick.AddListener(returnPage);
        process = new List<int[]>();
        solved = true;
        foreach (Transform board in transform)
        {
            foreach (Transform child in board.gameObject.transform)
            {
                grid.Add(child.gameObject);
            }
        }
        Run();
    }

    public void Run()
    {
        read();
    }

    void ButtonSolveFast()
    {
        solve(0);
        solved = true;
        sudoku = (int[,])mat.Clone();
        printSudoku(sudoku);
    }

    void ButtonSolveSlow()
    {
        solve(0);
        solved = true;
        printSudoku(sudoku);
        StartCoroutine(slowPrint(time / (float)process.Count));
        sudoku = (int[,])mat.Clone();
    }

    void ButtonRandomize()
    {
        randomize();
        printSudoku(sudoku);
    }

    void returnPage()
    {
        select.SetActive(true);
        gameObject.SetActive(false);
    }

    IEnumerator slowPrint(float single)
    {
        if (single == 0)
        {
            process.Clear();
            single = 3.0f / (float)(syx * syx);
            int count = 0;
            for (int y = 0; y < syx; y++)
            {
                for (int x = 0; x < syx; x++)
                {
                    process.Add(new int[] { count / syx, count % syx, mat[y, x] });
                    count++;
                }
            }
        }
        foreach (int[] p in process)
        {
            grid[(p[0] * syx) + p[1]].GetComponent<UnityEngine.UI.Text>().text = p[2].ToString();
            yield return new WaitForSeconds(single);
        }
        printSudoku(mat);
        process.Clear();
    }

    void LockInput()
    {
        if (input.text.Length > 0)
        {
            time = Convert.ToInt32(input.text);
        }
        else if (input.text.Length == 0)
        {
            input.text = "0";
            time = 0;
        }
    }

    void read()
    {
        string[] file = System.IO.File.ReadAllLines("Assets\\" + filedir + ".txt");
        int count = 0;
        foreach (string line in file)
        {
            if (line.Contains("x"))
            {
                string[] s = line.Split('x');
                sx = Convert.ToInt32(s[0]);
                sy = Convert.ToInt32(s[1]);
                syx = sx * sy;
                sudoku = new int[syx, syx];
                mat = new int[syx, syx];
            }
            else
            {
                foreach (string n in line.Split())
                {
                    sudoku[count / syx, count % syx] = Convert.ToInt32(n);
                    grid[count].GetComponent<UnityEngine.UI.Text>().text = n.ToString();
                    count++;
                }
            }
        }
    }

    bool validate(int y, int x, int n)
    {
        for (int num = 0; num < sy * sx; num++)
        {
            if (n == sudoku[y, num])
                return false;
            if (n == sudoku[num, x])
                return false;
        }

        int y0 = (y / sy) * sy;
        int x0 = (x / sx) * sx;
        for (int i = 0; i < sy; i++)
        {
            for (int j = 0; j < sx; j++)
            {
                if (n == sudoku[y0 + i, x0 + j])
                    return false;
            }
        }
        return true;
    }

    int solve(int y0)
    {
        int solvable = 0;
        for (int y = y0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                if (sudoku[y, x] == 0)
                {
                    for (int n = 1; n < syx + 1; n++)
                    {
                        if (validate(y, x, n))
                        {
                            //grid[(y * syx) + x].GetComponent<UnityEngine.UI.Text>().text = n.ToString();
                            if (solved)
                                process.Add(new int[] { y, x, n });
                            sudoku[y, x] = n;
                            solvable += solve(y);
                            sudoku[y, x] = 0;
                        }
                    }
                    return solvable;
                }
            }
        }
        solved = false;
        mat = (int[,])sudoku.Clone();
        return solvable + 1;
    }

    void randomize()
    {
        numbers();
        rows();
        cols();
        rows2();
        cols2();
    }


    void numbers()
    {
        for (int n = 1; n < syx; n++)
        {
            int rn = UnityEngine.Random.Range(1, syx);
            for (int y = 0; y < syx; y++)
            {
                for (int x = 0; x < syx; x++)
                {
                    if (sudoku[y, x] == n)
                        sudoku[y, x] = rn;
                    else if (sudoku[y, x] == rn)
                        sudoku[y, x] = n;
                }
            }
        }
    }

    void swapRows(int n1, int n2)
    {
        for (int i = 0; i < syx; i++)
        {
            int row = sudoku[n1, i];
            sudoku[n1, i] = sudoku[n2, i];
            sudoku[n2, i] = row;
        }
    }

    void swapCols(int n1, int n2)
    {
        for (int i = 0; i < syx; i++)
        {
            int row = sudoku[i, n1];
            sudoku[i, n1] = sudoku[i, n2];
            sudoku[i, n2] = row;
        }
    }

    void rows()
    {
        for (int n = 0; n < syx; n++)
        {
            int rn = UnityEngine.Random.Range(0, sy - 1);
            int y0 = (n / sy) * sy;
            swapRows(n, rn + y0);
        }
    }


    void cols()
    {
        for (int n = 0; n < syx; n++)
        {
            int rn = UnityEngine.Random.Range(0, sx - 1);
            int x0 = (n / sx) * sx;
            swapRows(n, rn + x0);
        }
    }


    void rows2()
    {
        for (int n = 0; n < sy; n++)
        {
            int rn = UnityEngine.Random.Range(0, sy - 1);
            for (int i = 0; i < sy; i++)
            {
                swapRows(n * sy + i, rn * sy + i);
            }
        }
    }

    void cols2()
    {
        for (int n = 0; n < sx; n++)
        {
            int rn = UnityEngine.Random.Range(0, sx - 1);
            for (int i = 0; i < sx; i++)
            {
                if (n * sx + i < syx && rn * sx + i < syx)
                    swapCols(n * sx + i, rn * sx + i);
            }
        }
    }

    void removeNumbers()
    {
        int[] items = Enumerable.Range(0, syx * syx).ToArray();
        for (int i = 0; i < items.Length - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, items.Length);
            int temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
        foreach (int m in items)
        {
            process.Clear();
            int n = sudoku[m / syx, m % syx];
            sudoku[m / syx, m % syx] = 0;
            if (solve(0) != 1)
                sudoku[m / syx, m % syx] = n;
            solved = true;
        }
        process.Clear();
        printSudoku(sudoku);
    }

    void printSudoku(int[,] mat)
    {
        for (int i = 0; i < syx; i++)
        {
            for (int j = 0; j < syx; j++)
            {
                grid[(i * syx) + j].GetComponent<UnityEngine.UI.Text>().text = mat[i, j].ToString();
            }
        }
    }
}
