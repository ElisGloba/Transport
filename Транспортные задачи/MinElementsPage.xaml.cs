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
    /// Логика взаимодействия для MinElementsPage.xaml
    /// </summary>
    public partial class MinElementsPage : Page
    {
        private int rows;
        private int columns;
        private TextBox[,] costInputBoxes;
        public MinElementsPage()
        {
            InitializeComponent();
        }

        private void CreateTable_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsInput.Text, out rows) && int.TryParse(ColumnsInput.Text, out columns) && rows > 0 && columns > 0)
            {
                InputTablePanel.Visibility = Visibility.Visible;
                CostMatrixGrid.Items.Clear();
                costInputBoxes = new TextBox[rows, columns];

                for (int i = 0; i < rows; i++)
                {
                    StackPanel rowPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };
                    for (int j = 0; j < columns; j++)
                    {
                        TextBox textBox = new TextBox
                        {
                            Width = 50,
                            Margin = new Thickness(5)
                        };
                        costInputBoxes[i, j] = textBox;
                        rowPanel.Children.Add(textBox);
                    }
                    CostMatrixGrid.Items.Add(rowPanel);
                }
            }
            else
            {
             MessageBox.Show("Пожалуйста, введите корректные положительные целые числа для строк и столбцов."); 
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] supply = SupplyInput.Text.Split(',').Select(int.Parse).ToArray();
                int[] demand = DemandInput.Text.Split(',').Select(int.Parse).ToArray();

                int totalSupply = supply.Sum();
                int totalDemand = demand.Sum();

                if (totalSupply != totalDemand)
                {
                    MessageBox.Show("Таблица несбалансирована.Пожалуйста, проверьте введенные данные."); 
                    return;
                }

                int[,] costs = new int[rows, columns];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)                    {
                        costs[i, j] = int.Parse(costInputBoxes[i,j].Text);
                    }
                }

                (int[,] allocation, int totalCost) =MinimumElementMethod(supply, demand, costs);
                ResultOutput.Text = FormatResult(allocation,totalCost);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private (int[,], int) MinimumElementMethod(int[] supply, int[] demand, int[,] costs)
        {
            int rows = supply.Length;
            int columns = demand.Length;
            int[,] allocation = new int[rows, columns];
            int totalCost = 0;

            while (supply.Sum() > 0 && demand.Sum() > 0)
            {
                int minCost = int.MaxValue;
                int minRow = -1, minCol = -1;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        
                        if (costs[i, j] < minCost && supply[i] > 0 && demand[j] > 0)
                        {
                            minCost = costs[i, j];
                            minRow = i;
                            minCol = j;
                        }
                    }
                }

                int allocationAmount = Math.Min(supply[minRow], demand[minCol]);
                allocation[minRow, minCol] = allocationAmount;
                totalCost += allocationAmount * costs[minRow, minCol];
                supply[minRow] -= allocationAmount;
                demand[minCol] -= allocationAmount;
            }

            return (allocation, totalCost);
        }

        private string FormatResult(int[,] result, int totalCost)
        {
            string output = "Итоговый план:\n";
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    output += $"{result[i, j]} ";
                }
                output += Environment.NewLine;
            }

            output += $"Общая стоимость: {totalCost}";
            return output;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}