using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace NpcEditor
{
    static public class DbMgr
    {
        private static string connectionString => ConfigurationManager.AppSettings["conString"];

        public static List<NpcInfo> NPC_List = new List<NpcInfo>();
        public static List<MapInfo> Map_List = new List<MapInfo>();


        public static void Init()
        {
            NPC_List = GetAllNpcs();
            Map_List = GetAllMap();
        }

        public static List<NpcInfo> GetAllNpcs()
        {
            List<NpcInfo> list = new List<NpcInfo>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Npc_Info";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader ResultDataReader = command.ExecuteReader())
                    {

                        while (ResultDataReader.Read())
                        {
                            NpcInfo item = new NpcInfo
                            {
                                ID = (int)ResultDataReader["ID"],
                                Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString()),
                                //Level = (int)ResultDataReader["Level"],
                                //Camp = (int)ResultDataReader["Camp"],
                                //Type = (int)ResultDataReader["Type"],
                                //Blood = (int)ResultDataReader["Blood"],
                                X = (int)ResultDataReader["X"],
                                Y = (int)ResultDataReader["Y"],
                                Width = (int)ResultDataReader["Width"],
                                Height = (int)ResultDataReader["Height"],
                                //MoveMin = (int)ResultDataReader["MoveMin"],
                                //MoveMax = (int)ResultDataReader["MoveMax"],
                                //BaseDamage = (int)ResultDataReader["BaseDamage"],
                                //BaseGuard = (int)ResultDataReader["BaseGuard"],
                                //Attack = (int)ResultDataReader["Attack"],
                                //Defence = (int)ResultDataReader["Defence"],
                                //Agility = (int)ResultDataReader["Agility"],
                                //Lucky = (int)ResultDataReader["Lucky"],
                                //MagicAttack = (int)ResultDataReader["MagicAttack"],
                                //MagicDefence = (int)ResultDataReader["MagicDefence"],
                                ModelID = ((ResultDataReader["ModelID"] == null) ? "" : ResultDataReader["ModelID"].ToString()),
                                ResourcesPath = ((ResultDataReader["ResourcesPath"] == null) ? "" : ResultDataReader["ResourcesPath"].ToString()),
                                //DropRate = ((ResultDataReader["DropRate"] == null) ? "" : ResultDataReader["DropRate"].ToString()),
                                //Experience = (int)ResultDataReader["Experience"],
                                //Delay = (int)ResultDataReader["Delay"],
                                //Immunity = (int)ResultDataReader["Immunity"],
                                //Alert = (int)ResultDataReader["Alert"],
                                //Range = (int)ResultDataReader["Range"],
                                //Preserve = (int)ResultDataReader["Preserve"],
                                Script = ((ResultDataReader["Script"] == null) ? "" : ResultDataReader["Script"].ToString()),
                                //FireX = (int)ResultDataReader["FireX"],
                                //FireY = (int)ResultDataReader["FireY"],
                                //DropId = (int)ResultDataReader["DropId"],
                                //CurrentBallId = (int)ResultDataReader["CurrentBallId"],
                                //speed = (int)ResultDataReader["speed"],
                                //Probability = (int)ResultDataReader["Probability"]
                            };
                            list.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Db Init Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return list;
        }

        public static List<MapInfo> GetAllMap()
        {
            List<MapInfo> list = new List<MapInfo>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Game_Map";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    using (SqlDataReader ResultDataReader = command.ExecuteReader())
                    {

                        while (ResultDataReader.Read())
                        {
                            MapInfo item = new MapInfo
                            {
                                BackMusic = (ResultDataReader["BackMusic"]?.ToString() ?? ""),
                                BackPic = (ResultDataReader["BackPic"]?.ToString() ?? ""),
                                BackroundHeight = (int)ResultDataReader["BackroundHeight"],
                                BackroundWidht = (int)ResultDataReader["BackroundWidht"],
                                DeadHeight = (int)ResultDataReader["DeadHeight"],
                                DeadPic = (ResultDataReader["DeadPic"]?.ToString() ?? ""),
                                DeadWidth = (int)ResultDataReader["DeadWidth"],
                                Description = (ResultDataReader["Description"]?.ToString() ?? ""),
                                DragIndex = (int)ResultDataReader["DragIndex"],
                                ForegroundHeight = (int)ResultDataReader["ForegroundHeight"],
                                ForegroundWidth = (int)ResultDataReader["ForegroundWidth"],
                                ForePic = (ResultDataReader["ForePic"]?.ToString() ?? ""),
                                ID = (int)ResultDataReader["ID"],
                                Name = (ResultDataReader["Name"]?.ToString() ?? ""),
                                Pic = (ResultDataReader["Pic"]?.ToString() ?? ""),
                                Remark = (ResultDataReader["Remark"]?.ToString() ?? ""),
                                Weight = (int)ResultDataReader["Weight"],
                                PosX = (ResultDataReader["PosX"]?.ToString() ?? ""),
                                PosX1 = (ResultDataReader["PosX1"]?.ToString() ?? ""),
                                Type = (byte)(int)ResultDataReader["Type"]
                            };
                            list.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Db Init Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return list;
            }
        }
        public static bool UpdateNpcHitbox(NpcInfo npc)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Npc_Info SET X = @X, Y = @Y, Width = @Width, Height = @Height WHERE ID = @ID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@X", npc.X);
                command.Parameters.AddWithValue("@Y", npc.Y);
                command.Parameters.AddWithValue("@Width", npc.Width);
                command.Parameters.AddWithValue("@Height", npc.Height);
                command.Parameters.AddWithValue("@ID", npc.ID);

                try
                {
                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("NPC Hitbox Update Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return result;
            }
        }
    }
}
