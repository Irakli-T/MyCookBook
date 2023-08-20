using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace MyCookBook
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CookBook"].ConnectionString);

            sqlConnection.Open();

            if (sqlConnection.State == ConnectionState.Open)
            {
                MessageBox.Show("Подключение установлено");
            }
        }

        private void buttonProductRating_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand(
                     "INSERT INTO ProductRating (Ingredients, Belki, Zhiri, Yglevodi, Kalorii) VALUES (@Ingredients, @Belki, @Zhiri, @Yglevodi, @Kalorii)",
                     sqlConnection);

            command.Parameters.AddWithValue("@Ingredients", textBoxProductRating.Text);
            command.Parameters.AddWithValue("@Belki", textBoxBelki.Text);
            command.Parameters.AddWithValue("@Zhiri", textBoxZhiri.Text);
            command.Parameters.AddWithValue("@Yglevodi", textBoxYglevodi.Text);
            command.Parameters.AddWithValue("@Kalorii", textBoxKalorii.Text);
            command.ExecuteNonQuery();


            MessageBox.Show("Данные успешно добавлены в таблицы.");
        }

        private void buttonInsertRecept_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand(
                    "INSERT INTO Dish (NameDish, ViewDish, CookingMethod) VALUES (@NameDish, @ViewDish, @CookingMethod)",
                    sqlConnection);

            command.Parameters.AddWithValue("@NameDish", textBoxNameDish.Text);
            command.Parameters.AddWithValue("@ViewDish", textBoxViewDish.Text);
            command.Parameters.AddWithValue("@CookingMethod", richTextBox1.Text);

            int rowsAffected1 = command.ExecuteNonQuery();

            SqlCommand command2 = new SqlCommand(
                "INSERT INTO Ingredients (NameDish, Ingredients, Quantity) VALUES (@NameDish, @Ingredients, @Quantity)",
                sqlConnection);

            command2.Parameters.AddWithValue("@NameDish", textBoxNameDish.Text);
            command2.Parameters.AddWithValue("@Ingredients", textBoxI1.Text);
            command2.Parameters.AddWithValue("@Quantity", textBoxQ1.Text);
            int rowsAffected2 = command2.ExecuteNonQuery();

            if (!string.IsNullOrEmpty(textBoxI2.Text) && !string.IsNullOrEmpty(textBoxQ2.Text))
            {
                command2.Parameters["@Ingredients"].Value = textBoxI2.Text;
                command2.Parameters["@Quantity"].Value = textBoxQ2.Text;
                int rowsAffected3 = command2.ExecuteNonQuery();
            }

            if (!string.IsNullOrEmpty(textBoxI3.Text) && !string.IsNullOrEmpty(textBoxQ3.Text))
            {
                command2.Parameters["@Ingredients"].Value = textBoxI3.Text;
                command2.Parameters["@Quantity"].Value = textBoxQ3.Text;
                int rowsAffected4 = command2.ExecuteNonQuery();
            }

            if (!string.IsNullOrEmpty(textBoxI4.Text) && !string.IsNullOrEmpty(textBoxQ4.Text))
            {
                command2.Parameters["@Ingredients"].Value = textBoxI4.Text;
                command2.Parameters["@Quantity"].Value = textBoxQ4.Text;
                int rowsAffected5 = command2.ExecuteNonQuery();
            }

            if (!string.IsNullOrEmpty(textBoxI5.Text) && !string.IsNullOrEmpty(textBoxQ5.Text))
            {
                command2.Parameters["@Ingredients"].Value = textBoxI5.Text;
                command2.Parameters["@Quantity"].Value = textBoxQ5.Text;
                int rowsAffected6 = command2.ExecuteNonQuery();
            }

            MessageBox.Show("Данные успешно добавлены в таблицы.");
        }

        private void buttonRecipe_Click(object sender, EventArgs e)
        {
            string dishName = textBoxReNameDish.Text;

            if (!string.IsNullOrEmpty(dishName))
            {
                using (SqlCommand command = new SqlCommand(
                    "SELECT CookingMethod FROM Dish WHERE NameDish = @DishName",
                    sqlConnection))
                {
                    command.Parameters.AddWithValue("@DishName", dishName);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string cookingMethod = reader["CookingMethod"].ToString();
                        richTextBox2.Text = cookingMethod;
                    }
                    else
                    {
                        richTextBox2.Text = "Блюдо не найдено.";
                    }
                    reader.Close();
                }

                // Здесь начинается код для обновления ListView
                listView1.Items.Clear();
                double totalBelki = 0; // Переменная для хранения суммы значений Belki
                double totalZhiri = 0; // Переменная для хранения суммы значений Zhiri
                double totalYglevodi = 0;
                double totalKalorii = 0;

                using (SqlCommand sqlCommand = new SqlCommand(
                    "SELECT i.Ingredients, i.Quantity, pr.Belki, pr.Zhiri, pr.Yglevodi, pr.Kalorii " +
                    "FROM Ingredients i " +
                    "JOIN ProductRating pr ON i.Ingredients = pr.Ingredients " +
                    "WHERE i.NameDish = @DishName",
                    sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@DishName", dishName);
                    SqlDataReader dataReader = null;

                    try
                    {
                        dataReader = sqlCommand.ExecuteReader();

                        while (dataReader.Read())
                        {
                            ListViewItem item = new ListViewItem(new string[] {
                    dataReader["Ingredients"].ToString(),
                    dataReader["Quantity"].ToString()

                });

                            double belki = Convert.ToDouble(dataReader["Belki"]);
                            double zhiri = Convert.ToDouble(dataReader["Zhiri"]);
                            double yglevodi = Convert.ToDouble(dataReader["Yglevodi"]);
                            double kalorii = Convert.ToDouble(dataReader["Kalorii"]);
                            double quantity = Convert.ToDouble(dataReader["Quantity"]);
                            double calculatedValue = (quantity / 100) * belki;
                            double calculatedZhiri = (quantity / 100) * zhiri;
                            double calculatedYglevodi = (quantity / 100) * yglevodi;
                            double calculatedKalorii = (quantity / 100) * kalorii;

                            totalBelki += calculatedValue; // Добавляем значение к сумме
                            totalZhiri += calculatedZhiri; // Добавляем значение к сумме Zhiri
                            totalYglevodi += calculatedYglevodi;
                            totalKalorii += calculatedKalorii;

                            listView1.Items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        if (dataReader != null && !dataReader.IsClosed)
                        {
                            dataReader.Close();
                        }
                    }
                }
                // После завершения цикла, выводим сумму в textBoxB
                textBoxReB.Text = totalBelki.ToString();
                textBoxReZh.Text = totalZhiri.ToString();
                textBoxReYg.Text = totalYglevodi.ToString();
                textBoxReK.Text = totalKalorii.ToString();
            }
            else
            {
                richTextBox2.Text = "Введите название блюда.";
            }
        }
    }
}
