using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using System.Data;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BlazorServerAuthenticationAndAuthorization.Data
{
    public class DB : DbContext
    {
        private IConfiguration config;
        public DB(IConfiguration configuration)
        {
            config = configuration;
        }

        private string ConnectionString
        {
            get
            {
                string _server = config.GetValue<string>("DbConfig:ServerName");
                string _database = config.GetValue<string>("DbConfig:DatabaseName");
                string _username = config.GetValue<string>("DbConfig:UserName");
                return $"server={_server};username={_username};database={_database}";
            }
        }

        // Список аккаунтов (Клиенты)
        public async Task<List<DBItems>> GetClientsFromDB()
        {
            List<DBItems> items = new();
            DBItems currentItem;
            DataTable dataTable = new();
            MySqlConnection connection = new(ConnectionString);
            connection.Open();
            MySqlDataAdapter dataAdpter = new("SELECT * FROM клиент", connection);
            dataAdpter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                currentItem = new DBItems
                {
                    Id = Convert.ToInt32(row["idКлиента"]),
                    Login = row["Логин"] as string,
                    Password = row["Пароль"] as string,
                    FullName = row["ФИО"] as string,
                    CarStateNumber = row["Номер машины"] as string,
                    CarBrand = row["Марка машины"] as string,
                    PhoneNumber = row["Номер телефона"] as string,
                    IsAdmin = Convert.ToBoolean(row["Права администратора"])
                };
                items.Add(currentItem);
            }
            connection.Close();
            return await Task.FromResult(items);
        }

        // Список заказов (Клиенты/Услуги)
        public async Task<List<DBItems>> GetProvidedOrderListFromDB()
        {
            List<DBItems> items = new();
            DBItems currentItem;
            DataTable dataTable = new();
            MySqlConnection connection = new(ConnectionString);
            connection.Open();
            MySqlDataAdapter dataAdpter = new("SELECT * FROM `клиенты/услуги`", connection);
            dataAdpter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                currentItem = new DBItems
                {
                    Id = Convert.ToInt32(row["idКлиенты/Услуги"]),
                    IdClient = Convert.ToInt32(row["idКлиента"]),
                    IdOrder = Convert.ToInt32(row["idУслуги"]),
                    IdMaterial = Convert.ToInt32(row["Код материала"]),
                    DateOfReceipt = row["Дата приёма"] as string,
                    DateOfCompletion = row["Дата выполнения"] as string
                };
                items.Add(currentItem);
            }
            connection.Close();
            return await Task.FromResult(items);
        }

        // Список материалов (Материалы)
        public async Task<List<DBItems>> GetMaterialsFromDB()
        {
            List<DBItems> items = new();
            DBItems currentItem;
            DataTable dataTable = new();
            MySqlConnection connection = new(ConnectionString);
            connection.Open();
            MySqlDataAdapter dataAdpter = new("SELECT * FROM материалы", connection);
            dataAdpter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                currentItem = new DBItems
                {
                    Id = Convert.ToInt32(row["idМатериала"]),
                    MaterialName = row["Материал"] as string,
                    MaterialCost = row["Стоимость материала"] as string,
                };
                items.Add(currentItem);
            }
            connection.Close();
            return await Task.FromResult(items);
        }

        // Список услуг (Услуги)
        public async Task<List<DBItems>> GetOrderListFromDB()
        {
            List<DBItems> items = new();
            DBItems currentItem;
            DataTable dataTable = new();
            MySqlConnection connection = new(ConnectionString);
            connection.Open();
            MySqlDataAdapter dataAdpter = new("SELECT * FROM услуги", connection);
            dataAdpter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                currentItem = new DBItems
                {
                    Id = Convert.ToInt32(row["idУслуги"]),
                    OrderName = row["Услуга"] as string,
                    OrderPrice = Convert.ToInt32(row["Стоимость"]),
                    Guarantee = Convert.ToInt32(row["Гарантия (месяцев)"])
                };
                items.Add(currentItem);
            }
            connection.Close();
            return await Task.FromResult(items);
        }
        public async Task Delete(string table, int id)
        {
            MySqlConnection connection = new(ConnectionString);
            connection.Open();

            string sId = "";
            switch(table)
            {
                case "клиент":
                    sId = "idКлиента";
                    break;
                case "клиенты/услуги":
                    sId = "idКлиенты/Услуги";
                    break;
                case "материалы":
                    sId = "idМатериала";
                    break;
                case "услуги":
                    sId = "idУслуги";
                    break;
            }
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM `{table}` WHERE `{sId}` = {id}";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
        public async Task Add(string table, string[] newItem)
        {
            MySqlConnection connection = new(ConnectionString);
            try
            {
                MySqlCommand cmd = connection.CreateCommand();
                List<DBItems> dBItems;
                connection.Open();

                switch (table)
                {
                    case "клиент":     
                        dBItems = new List<DBItems>();
                        dBItems = await GetClientsFromDB();
                        cmd.CommandText = $"INSERT `{table}` VALUES ('{newItem[0]}', '{newItem[1]}', '{newItem[2]}', '{newItem[3]}', '{newItem[4]}', '{newItem[5]}', '{newItem[6]}', '{Convert.ToBoolean(newItem[7])}')";
                        break;
                    case "клиенты/услуги":
                        dBItems = new List<DBItems>();
                        dBItems = await GetProvidedOrderListFromDB();
                        cmd.CommandText = $"INSERT `{table}` VALUES ('{newItem[0]}', '{newItem[1]}', '{newItem[2]}', '{newItem[3]}', '{newItem[4]}', '{newItem[5]}')";
                        break;
                    case "материалы":
                        dBItems = new List<DBItems>();
                        dBItems = await GetMaterialsFromDB();
                        cmd.CommandText = $"INSERT `{table}` VALUES ('{newItem[0]}', '{newItem[1]}', '{newItem[2]}')";
                        break;
                    case "услуги":
                        dBItems = new List<DBItems>();
                        dBItems = await GetOrderListFromDB();
                        cmd.CommandText = $"INSERT `{table}` VALUES ('{newItem[0]}', '{newItem[1]}', '{newItem[2]}', '{newItem[3]}')";
                        break;
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }

        }
        public async Task Reg(BlazorServerAuthenticationAndAuthorization.Pages.Register.Model model)
        {
            MySqlConnection connection = new(ConnectionString);
            try
            {
                MySqlCommand cmd = connection.CreateCommand();
                connection.Open();
                cmd.CommandText = $"INSERT `клиент` VALUES ('', '{model.UserName}', '{model.Password}', '{model.FullName}', '{model.CarStateNumber}', '{model.CarBrand}', '{model.PhoneNumber}', false)";             
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }
        private string[] ColumnName = new string[10];
        public async Task Change(string table, DBItems newItem, int oldItemId)
        {
            List<DBItems> dBItems;
            GetColumnName(table).Wait();
            MySqlConnection connection = new(ConnectionString);
            MySqlCommand cmd = connection.CreateCommand();
            connection.Open();
            
            try
            {
                switch (table)
                {
                    case "клиент":
                        dBItems = await GetClientsFromDB();
                        cmd.CommandText = $"UPDATE `{table}` SET `{ColumnName[0]}`={newItem.Id}, `{ColumnName[1]}`='{newItem.Login}', `{ColumnName[2]}`='{newItem.Password}', `{ColumnName[3]}`='{newItem.FullName}', `{ColumnName[4]}`='{newItem.CarStateNumber}', `{ColumnName[5]}`='{newItem.CarBrand}', `{ColumnName[6]}`='{newItem.PhoneNumber}', `{ColumnName[7]}`={Convert.ToBoolean(newItem.IsAdmin)} WHERE `{ColumnName[0]}`={oldItemId}";
                        break;
                    case "клиенты/услуги":
                        dBItems = await GetProvidedOrderListFromDB();
                        cmd.CommandText = $"UPDATE `{table}` SET `{ColumnName[0]}`={newItem.Id}, `{ColumnName[1]}`={newItem.IdClient}, `{ColumnName[2]}`={newItem.IdOrder}, `{ColumnName[3]}`={newItem.IdMaterial},`{ColumnName[4]}`='{newItem.DateOfReceipt}', `{ColumnName[3]}`='{newItem.DateOfCompletion}' WHERE `{ColumnName[0]}`={oldItemId}";
                        break;
                    case "материалы":
                        dBItems = await GetMaterialsFromDB();
                        cmd.CommandText = $"UPDATE `{table}` SET `{ColumnName[0]}`={newItem.Id}, `{ColumnName[1]}`='{newItem.MaterialName}', `{ColumnName[2]}`='{newItem.MaterialCost}' WHERE `{ColumnName[0]}`={oldItemId}";
                        break;
                    case "услуги":
                        dBItems = await GetOrderListFromDB();
                        cmd.CommandText = $"UPDATE `{table}` SET `{ColumnName[0]}`={newItem.Id}, `{ColumnName[1]}`='{newItem.OrderName}', `{ColumnName[2]}`={newItem.OrderPrice}, `{ColumnName[2]}`={newItem.Guarantee} WHERE `{ColumnName[0]}`={oldItemId}";
                        break;
                }
                cmd.ExecuteNonQuery();
;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }
        private async Task GetColumnName(string table)
        {
            MySqlConnection connection = new(ConnectionString);
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM `{table}` LIMIT 0";
            connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            try
            {
                var columnSchema = reader.GetColumnSchema();

                for (int i = 0; i < reader.FieldCount; ++i)
                    ColumnName[i] = columnSchema[i].ColumnName;

                await reader.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // Добавить заказ в столбец "Материал"
        public async Task SetOrderForMaterials(DBItems item, string date, int clientId)
        {
            MySqlConnection connection = new(ConnectionString);
            connection.Open();
            MySqlCommand cmd = connection.CreateCommand();
            try
            {
                cmd.CommandText = $"INSERT `клиенты/услуги` VALUES ('', '{clientId}', '0', '{item.Id}', '{date}', '0')";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // Добавить заказ в столбец "Услуга"
        public async Task SetOrderForServices(DBItems item, string date, int clientId)
        {
            MySqlConnection connection = new(ConnectionString);
            connection.Open();
            MySqlCommand cmd = connection.CreateCommand();
            try
            {
                cmd.CommandText = $"INSERT `клиенты/услуги` VALUES ('', '{clientId}', '{item.Id}', '0', '{date}', '0')";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
