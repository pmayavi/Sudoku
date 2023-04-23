using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class SudokuScript : MonoBehaviour
{
    private List<GameObject> grid = new List<GameObject>();
    private int sy;
    private int sx;
    private int syx;
    private List<int>[,] sudoku;
    private int[,] mat;
    private bool morethantwo;
    private List<int[]> process;
    private bool solved;
    private bool exitAuto;
    public Button buttonSolveFast;
    public Button buttonSolveSlow;
    public Button buttonRandomize;
    public Button buttonRemove;
    public Button returns;
    public Button auto;
    public Text timeTaken;
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
        buttonRemove.onClick.AddListener(ButtonRemoveNumbers);
        returns.onClick.AddListener(returnPage);
        if (auto)
            auto.onClick.AddListener(automate);
        process = new List<int[]>();
        solved = true;
        morethantwo = false;
        foreach (Transform board in transform)
        {
            foreach (Transform child in board.gameObject.transform)
            {
                grid.Add(child.gameObject);
            }
        }
        read();
    }

    void Update()
    {
        if (exitAuto)
        {
            if (solved)
            {
                randomize();
                removeNumbers();
                ButtonSolveSlow();
            }
        }
    }

    void ButtonSolveFast()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        long currentTime = watch.ElapsedMilliseconds;
        marker();
        timeTaken.text = ((float)(watch.ElapsedMilliseconds - currentTime) / 1000.0).ToString() + " Segundos";
        morethantwo = false;
        solved = true;
        matrixToSudoku();
        printSudoku();
    }

    void ButtonSolveSlow()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        long currentTime = watch.ElapsedMilliseconds;
        marker();
        timeTaken.text = ((float)(watch.ElapsedMilliseconds - currentTime) / 1000.0).ToString() + " Segundos";
        morethantwo = false;
        printSudoku();
        StartCoroutine(slowPrint(time / (float)process.Count));
        matrixToSudoku();
    }

    void ButtonRandomize()
    {

        var watch = System.Diagnostics.Stopwatch.StartNew();
        long currentTime = watch.ElapsedMilliseconds;
        marker();
        randomize();
        timeTaken.text = ((float)(watch.ElapsedMilliseconds - currentTime) / 1000.0).ToString() + " Segundos";
        printSudoku();
    }

    void ButtonRemoveNumbers()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        long currentTime = watch.ElapsedMilliseconds;
        removeNumbers();
        timeTaken.text = ((float)(watch.ElapsedMilliseconds - currentTime) / 1000.0).ToString() + " Segundos";
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
        solved = true;
        matrixToSudoku();
        printSudoku();
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

    void automate()
    {
        exitAuto = !exitAuto;
    }

    public void read()
    {
        string[] file = System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/" + filedir + ".txt");
        int count = 0;
        foreach (string line in file)
        {
            if (line.Contains("x"))
            {
                string[] s = line.Split('x');
                sx = Convert.ToInt32(s[0]);
                sy = Convert.ToInt32(s[1]);
                syx = sx * sy;
                sudoku = new List<int>[syx, syx];
                mat = new int[syx, syx];
            }
            else
            {
                foreach (string n in line.Split())
                {
                    sudoku[count / syx, count % syx] = new List<int>();
                    sudoku[count / syx, count % syx].Add(Convert.ToInt32(n));
                    grid[count].GetComponent<UnityEngine.UI.Text>().text = n;
                    count++;
                }
            }
        }
    }

    bool validate(int y, int x, int n)
    {
        for (int num = 0; num < sy * sx; num++)
        {
            if (n == sudoku[y, num][0])
                return false;
            if (n == sudoku[num, x][0])
                return false;
        }

        int y0 = (y / sy) * sy;
        int x0 = (x / sx) * sx;
        for (int i = 0; i < sy; i++)
        {
            for (int j = 0; j < sx; j++)
            {
                if (n == sudoku[y0 + i, x0 + j][0])
                    return false;
            }
        }
        return true;
    }


    public int marker()
    {
        if (morethantwo)
            return 0;
        int solvable = 0;

        for (int y = 0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                if (sudoku[y, x][0] == 0 && sudoku[y, x].Count == 1)
                {
                    for (int n = 1; n < syx + 1; n++)
                    {
                        if (validate(y, x, n))
                        {
                            sudoku[y, x].Add(n);
                        }
                    }
                    solvable += marker();
                    sudoku[y, x] = new List<int>();
                    sudoku[y, x].Add(0);
                    return solvable;
                }
            }
        }
        return singles();
    }

    int singles()
    {
        if (morethantwo)
            return 0;
        int solvable = 0;

        for (int y = 0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                if (sudoku[y, x][0] == 0)
                {
                    if (sudoku[y, x].Count < 2)
                    {
                        return solvable;
                    }
                    if (sudoku[y, x].Count == 2)
                    {
                        sudoku[y, x].RemoveAt(0);
                        if (solved)
                            process.Add(new int[] { y, x, sudoku[y, x][0] });
                        List<int[]> removed = cleanup();
                        solvable += singles();
                        undo(removed);
                        int temp = sudoku[y, x][0];
                        sudoku[y, x][0] = 0;
                        sudoku[y, x].Add(temp);
                        return solvable;
                    }
                }
            }
        }
        return doubles(0);
    }

    int doubles(int y0)
    {
        if (morethantwo)
            return 0;
        int solvable = 0;

        for (int y = 0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                if (sudoku[y, x][0] == 0 && sudoku[y, x].Count < sx)
                {
                    int last = sudoku[y, x][0];
                    sudoku[y, x].RemoveAt(0);
                    sudoku[y, x].Add(last);
                    List<int[]> removed = cleanup();

                    while (sudoku[y, x][0] != 0)
                    {
                        if (solved)
                            process.Add(new int[] { y, x, sudoku[y, x][0] });
                        solvable += singles();
                        undo(removed);
                        last = sudoku[y, x][0];
                        sudoku[y, x].RemoveAt(0);
                        sudoku[y, x].Add(last);
                        removed = cleanup();
                    }
                    return solvable;
                }
            }
        }
        return solve(0);
    }

    int solve(int y0)
    {
        if (morethantwo)
            return 0;
        int solvable = 0;

        for (int y = y0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                if (sudoku[y, x][0] == 0)
                {
                    int last = sudoku[y, x][0];
                    sudoku[y, x].RemoveAt(0);
                    sudoku[y, x].Add(last);
                    List<int[]> removed = cleanup();

                    while (sudoku[y, x][0] != 0)
                    {
                        //grid[(y * syx) + x].GetComponent<UnityEngine.UI.Text>().text = n.ToString();
                        solvable += singles();
                        undo(removed);
                        last = sudoku[y, x][0];
                        sudoku[y, x].RemoveAt(0);
                        sudoku[y, x].Add(last);
                        removed = cleanup();
                    }
                    return solvable;
                }
            }
        }

        if (solvable == 0)
        {
            solved = false;
            saveSudoku();
        }
        else if (solvable > 1)
        {
            morethantwo = true;
        }
        return solvable + 1;
    }

    List<int[]> cleanup()
    {
        List<int[]> removed = new List<int[]>();
        for (int y = 0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                if (sudoku[y, x][0] == 0)
                {
                    foreach (int n in sudoku[y, x])
                    {
                        if (n != 0 && !validate(y, x, n))
                        {
                            removed.Add(new int[] { y, x, n });
                        }
                    }
                }
            }
        }
        foreach (int[] z in removed)
        {
            sudoku[z[0], z[1]].Remove(z[2]);
        }
        return removed;
    }

    void undo(List<int[]> removed)
    {
        foreach (int[] z in removed)
        {
            sudoku[z[0], z[1]].Add(z[2]);
        }
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
                    if (sudoku[y, x][0] == n)
                        sudoku[y, x][0] = rn;
                    else if (sudoku[y, x][0] == rn)
                        sudoku[y, x][0] = n;
                }
            }
        }
    }

    void swapRows(int n1, int n2)
    {
        for (int i = 0; i < syx; i++)
        {
            int row = sudoku[n1, i][0];
            sudoku[n1, i][0] = sudoku[n2, i][0];
            sudoku[n2, i][0] = row;
        }
    }

    void swapCols(int n1, int n2)
    {
        for (int i = 0; i < syx; i++)
        {
            int row = sudoku[i, n1][0];
            sudoku[i, n1][0] = sudoku[i, n2][0];
            sudoku[i, n2][0] = row;
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
            swapCols(n, rn + x0);
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

        var watch = System.Diagnostics.Stopwatch.StartNew();
        long time = 0;
        int limit = (sy + sx) * 1000;
        foreach (int m in items)
        {
            int n = sudoku[m / syx, m % syx][0];
            if (n != 0)
            {
                process.Clear();
                time = watch.ElapsedMilliseconds;
                sudoku[m / syx, m % syx][0] = 0;
                if (marker() != 1)
                    sudoku[m / syx, m % syx][0] = n;
                if (watch.ElapsedMilliseconds - time > limit) break;
                solved = true;
                morethantwo = false;
            }
        }
        process.Clear();
        watch.Stop();
        printSudoku();
    }

    public void matrixToSudoku()
    {
        sudoku = new List<int>[syx, syx];
        for (int y = 0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                sudoku[y, x] = new List<int>();
                sudoku[y, x].Add(mat[y, x]);
            }
        }
    }

    public void saveSudoku()
    {
        mat = new int[syx, syx];
        for (int y = 0; y < syx; y++)
        {
            for (int x = 0; x < syx; x++)
            {
                mat[y, x] = sudoku[y, x][0];
            }
        }
    }

    void printSudoku()
    {
        for (int i = 0; i < syx; i++)
        {
            for (int j = 0; j < syx; j++)
            {
                grid[(i * syx) + j].GetComponent<UnityEngine.UI.Text>().text = sudoku[i, j][0].ToString();
            }
        }
    }
}
