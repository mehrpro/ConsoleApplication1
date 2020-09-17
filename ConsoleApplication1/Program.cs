using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using SmsIrRestful;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var lii = ReaderSQL();
            //var result = RegisterINSqlServer(lii);

            var result = SendSMS(09186620474, "18542", "1398/12/12");
        }

        public class RegisteredTagList
        {
            public int ID { get; set; }
            public string Tagid { get; set; }
            public DateTime dateRegister { get; set; }
        }
        public static bool SendSMS(long mobile,  string OrderNumberValue, string orderDateValue)
        {
            var token = new Token().GetToken("17413e1864890dd130c73e17", "Fm&)**)!@(*");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = mobile,
                TemplateId = 33362,
                ParameterArray = new List<UltraFastParameters>()
                {
                    new UltraFastParameters() {Parameter = "orderNumber" , ParameterValue = OrderNumberValue,},
                    new UltraFastParameters() {Parameter = "orderDate",ParameterValue = orderDateValue}

                }.ToArray()

            };
            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool RegisterINSqlServer(List<RegisteredTagList> registeredTagLists)
        {
            using (var connSQL = new SqlConnection("data source=10.1.1.4;initial catalog=infraBase ;user id=sa;password=Ss987654;MultipleActiveResultSets=True;"))
            {
                try
                {
                    connSQL.Open();
                    foreach (var item in registeredTagLists)
                    {
                        var str =
                            $"insert into tagRec(TagID,DateTimeRegister,MysqlID) Values('{item.Tagid}','{item.dateRegister.Date}','{item.ID}')";
                        var cmd = new SqlCommand(str, connSQL);
                        var resly =   cmd.ExecuteNonQuery();
                    }
                    connSQL.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    var str = ex.Message;
                    connSQL.Close();
                    return false;
                }
            }
        }
        private static List<RegisteredTagList> ReaderSQL()
        {
            string cs = @"server=10.1.1.3;port=3306;userid=fm;password=Ss987654;database=schooldb;SSL Mode = None";
            var list = new List<RegisteredTagList>();
            using (var conn = new MySqlConnection(cs))
            {
                try
                {
                    conn.Open();
                    var cmdString = "SELECT * FROM schooldb.tagrecive where registered = 1";
                    var cmd = new MySqlCommand(cmdString, conn);
                    var result = cmd.ExecuteReader();
                    while (result.Read())
                    {
                        list.Add(new RegisteredTagList()
                        {
                            ID = result.GetInt32(0),
                            Tagid = result.GetString(1),
                            dateRegister = result.GetDateTime(2),
                        });
                    }
                    conn.Clone();
                    return list;
                }
                catch
                {
                    conn.Clone();
                    return null;
                }
            }
        }
    }
}
