﻿using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using repositories.models;

namespace repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(MySqlConnection _connection, AppSecrets _appSecrets)
        {
            connection = _connection;
            if(connection.State == ConnectionState.Closed){
                connection.Open();
            }
            appSecrets = _appSecrets;
        }

        private MySqlConnection connection { get; set; }
        private AppSecrets appSecrets { get; set; }

        public User Find(string username, string password)
        {
            User user = new User();
            var query = $"SELECT * FROM `Users` WHERE Username = '{username}' AND Password = '{password}'";
            var command = new MySqlCommand(query, connection);
            command.CommandType = CommandType.Text;
            try
            {
                using(DbDataReader rdr = command.ExecuteReader()){
                    if(!rdr.HasRows) return null;
                    rdr.Read();
                    user.ID = rdr.GetFieldValue<int>(0);
                    user.Username = rdr.GetFieldValue<string>(1);
                    rdr.Close();
                }
            } catch (MySqlException ex) {
                Console.WriteLine(ex);
                // TODO: inspect exception and throw
                // custom MySqlTimeoutException if
                // connection timed out (broken pipe).
                // otherwise, allow the MySqlException
                // to bubble up
                return new User() { ID = 0 };
            }
            return user;
        }

        public async Task<bool> Exists(string username){
            var query = $"SELECT COUNT(ID) FROM `Users` WHERE Username = '{username}'";
            var command = new MySqlCommand(query, connection);
            bool exists = false;
            command.CommandType = CommandType.Text;
            using(DbDataReader rdr = await command.ExecuteReaderAsync()){
                rdr.Read();
                exists = rdr.GetFieldValue<long>(0) > 0;
            }
            return exists;
        }

        public async Task<bool> Create(string username, string password)
        {
            var cmdText = $"INSERT INTO `Users` (Username, Password) VALUES ('{username}','{password}')";
            var command = new MySqlCommand(cmdText, connection);
            command.CommandType = CommandType.Text;
            return await command.ExecuteNonQueryAsync() == 1;
        }

        public IUserRepository Reconnect(){
            connection = new MySqlConnection(appSecrets.MySQLConnectionString);
            connection.Open();
            return this;
        }

    }
}
