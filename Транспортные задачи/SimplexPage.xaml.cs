using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Транспортные_задачи
{
    /// <summary>
    /// Логика взаимодействия для SimplexPage.xaml
    /// </summary>
    public partial class SimplexPage : Page
    {
        private int variablesCount;
        private int constraintsCount;
        private TextBox[] coefficientsInputBoxes;
        private TextBox[,] constraintsInputBoxes;
        private TextBox[] limitsInputBoxes;
        public SimplexPage()
        {
            InitializeComponent();
        }

        private void CreateTable_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(VariablesInput.Text, out variablesCount) && int.TryParse(ConstraintsInput.Text, out constraintsCount) && variablesCount > 0 && constraintsCount > 0)
            {
                InputTablePanel.Visibility = Visibility.Visible;
                CoefficientsGrid.Items.Clear();
                ConstraintsGrid.Items.Clear();
                coefficientsInputBoxes = new TextBox[variablesCount];
                constraintsInputBoxes = new TextBox[constraintsCount,variablesCount];
                limitsInputBoxes = new TextBox[constraintsCount];

                StackPanel coefficientsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                for (int j = 0; j < variablesCount; j++)

                {
                    TextBox textBox = new TextBox
                    {
                        Width = 50,
                        Margin= new Thickness(5)
                    };
                    coefficientsInputBoxes[j] = textBox;
                    coefficientsPanel.Children.Add(textBox);
                }
                CoefficientsGrid.Items.Add(coefficientsPanel);

                for (int i = 0; i < constraintsCount; i++)
                {
                    StackPanel rowPanel = new StackPanel
                    {
                        Orientation= Orientation.Horizontal
                    };
                    for (int j = 0; j < variablesCount; j++)
                    {
                        TextBox textBox = new TextBox
                        {
                            Width = 50,
                            Margin = new Thickness(5)
                        };
                        constraintsInputBoxes[i, j] = textBox;
                        rowPanel.Children.Add(textBox);
                    }
                    TextBox limitTextBox = new TextBox
                    {
                        Width = 50,
                        Margin = new Thickness(5)
                    };
                    limitsInputBoxes[i] = limitTextBox;
                    rowPanel.Children.Add(limitTextBox);
                    ConstraintsGrid.Items.Add(rowPanel);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные положительные целые числа для переменных и ограничений."); 
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double[] coefficients = new double[variablesCount];
                for (int j = 0; j < variablesCount; j++)
                {
                    coefficients[j] =double.Parse(coefficientsInputBoxes[j].Text);
                }

                double[,] constraints = new double[constraintsCount,variablesCount];
                double[] limits = new double[constraintsCount];

                for (int i = 0; i < constraintsCount; i++)
                {
                    for (int j = 0; j < variablesCount; j++)
                    {
                        constraints[i, j] =double.Parse(constraintsInputBoxes[i, j].Text);
                    }
                    limits[i] =double.Parse(limitsInputBoxes[i].Text);
                }

                // Вызов симплекс-метода 
                (double[] result, double profit) = SimplexMethod(coefficients, constraints, limits);



                // Форматирование и отображение результата 
                ResultOutput.Text = FormatResult(result) + $"\nПрибыль: {profit}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private (double[] result, double profit)
SimplexMethod(double[] coefficients, double[,] constraints, double[]
limits)
        {
            int numVariables = coefficients.Length;
            int numConstraints = limits.Length;

            // Создание расширенной таблицы симплекс-метода 
            double[,] tableau = new double[numConstraints + 1,numVariables + numConstraints + 1];

            // Заполнение коэффициентов целевой функции 
            for (int j = 0; j < numVariables; j++)
            {
                tableau[0, j] = coefficients[j];
            }

            // Заполнение ограничений 
            for (int i = 0; i < numConstraints; i++)
            {
                for (int j = 0; j < numVariables; j++)
                {
                    tableau[i + 1, j] = constraints[i, j];
                }
                tableau[i + 1, numVariables + i] = 1;
                tableau[i + 1, tableau.GetLength(1) - 1] = limits[i];
            }

            while (true)
            {
                int pivotColumn = -1;
                double maxCoefficient = 0;
                for (int j = 0; j < tableau.GetLength(1) - 1; j++)
                {
                    if (tableau[0, j] > maxCoefficient)
                    {
                        maxCoefficient = tableau[0, j];
                        pivotColumn = j;
                    }
                }

                if (pivotColumn == -1)
                {
                    break;
                }

                int pivotRow = -1;
                double minRatio = double.MaxValue;
                for (int i = 1; i < tableau.GetLength(0); i++)
                {
                    if (tableau[i, pivotColumn] > 0)
                    {



                        double ratio = tableau[i, tableau.GetLength(1) - 1] / tableau[i, pivotColumn];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    throw new InvalidOperationException("Задача не ограничена."); 
                }

                // Обновление таблицы 
                double pivotValue = tableau[pivotRow, pivotColumn];
                for (int j = 0; j < tableau.GetLength(1); j++)
                {
                    tableau[pivotRow, j] /= pivotValue;
                }

                for (int i = 0; i < tableau.GetLength(0); i++)
                {
                    if (i != pivotRow)
                    {
                        double factor = tableau[i, pivotColumn];
                        for (int j = 0; j < tableau.GetLength(1); j++)
                        {
                            tableau[i, j] -= factor *tableau[pivotRow, j];
                        }
                    }
                }
            }

            double[] result = new double[numVariables];
            for (int i = 1; i < tableau.GetLength(0); i++)
            {
                bool isBasic = true;
                int basicVariableIndex = -1;
                for (int j = 0; j < numVariables; j++)
                {
                    if (tableau[i, j] == 1)
                    {
                        if (basicVariableIndex == -1)
                        {
                            basicVariableIndex = j;
                        }
                        else
                        {
                            isBasic = false;
                            break;
                        }
                    }
                    else if (tableau[i, j] != 0)
                    {
                        isBasic = false;
                        break;
                    }
                }
                if (isBasic && basicVariableIndex != -1)
                {
                    result[basicVariableIndex] = tableau[i,
                    tableau.GetLength(1) - 1];
                }
            }
            double profit = 0;
            for (int j = 0; j < numVariables; j++)
            {
                profit += coefficients[j] * result[j];
            }
            return (result, profit);
        }
        private string FormatResult(double[] result)
        {
            string output = "Оптимальные значения:\n";
            for (int i = 0; i < result.Length; i++)
            {
                output += $"x{i + 1} = {result[i]}\n";
            }
            return output;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}