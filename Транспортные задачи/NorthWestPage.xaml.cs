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
    /// Логика взаимодействия для NorthWestPage.xaml
    /// </summary>
        public partial class NorthWestPage : Page
        {
            private int rows;
            private int columns;
            private TextBox[,] costInputBoxes;

            public NorthWestPage()
            {
                InitializeComponent();
            }

            private void CreateTable_Click(object sender, RoutedEventArgs     e)
            {
                if (int.TryParse(RowsInput.Text, out rows) && 
                int.TryParse(ColumnsInput.Text, out columns) && rows > 0 && columns >  0)
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
                    int[] supply =
    SupplyInput.Text.Split(',').Select(int.Parse).ToArray();
                    int[] demand =
    DemandInput.Text.Split(',').Select(int.Parse).ToArray();

                    int totalSupply = supply.Sum();
                    int totalDemand = demand.Sum();

                    if (totalSupply != totalDemand)
                    {
                        MessageBox.Show("Таблица несбалансирована.Пожалуйста, проверьте введенные данные.");   

                    return; // Exit the method if the balance check fails
                    }

                    int[,] costs = new int[rows, columns];

                    // Чтение затрат из полей ввода 
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            costs[i, j] = int.Parse(costInputBoxes[i,  j].Text);
                        }
                    }

                    string result = NorthwestCorner(supply, demand,costs);
                    ResultOutput.Text = result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }

            private string NorthwestCorner(int[] supply, int[] demand, int[,] costs)
            {
                int[,] result = new int[supply.Length, demand.Length];
                int totalCost = 0;
                int supplyIndex = 0, demandIndex = 0;

                while (supplyIndex < supply.Length && demandIndex < demand.Length)
                {
                    // Определяем количество, которое можно выделить 
                    int allocation = Math.Min(supply[supplyIndex], demand[demandIndex]);
                    result[supplyIndex, demandIndex] = allocation;
                    totalCost += allocation * costs[supplyIndex, demandIndex];

                    // Уменьшаем оставшиеся поставки и спрос 
                    supply[supplyIndex] -= allocation;
                    demand[demandIndex] -= allocation;

                    // Переход к следующему поставщику, если все поставки выполнены
                    if (supply[supplyIndex] == 0)
                    {
                        supplyIndex++;
                    }

                    // Переход к следующему потребителю, если весь спрос выполнен
                    if (demand[demandIndex] == 0)
                    {
                        demandIndex++;
                    }
                }

                return FormatResult(result, totalCost);
    

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