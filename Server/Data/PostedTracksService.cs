using HVMDash.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
//using TableDependency.EventArgs;
//using TableDependency.SqlClient;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Data
{
    //public class PostedTracksService : IPostedTracksService, IDisposable
    //{
    //    private const string TableName = "PostedTracks";
    //    private SqlTableDependency<PostedTrack> _notifier;
    //    private IConfiguration _configuration;

    //    public event PostedTracksDelegate OnPostedTracksChanged;

    //    //public PostedTracksService(IConfiguration configuration)
    //    //{
    //    //    _configuration = configuration;

    //    //    _notifier = new SqlTableDependency<PostedTrack>(_configuration["ConnectionString"], TableName);
    //    //    _notifier.OnChanged += this.TableDependency_Changed;
    //    //    _notifier.Start();
    //    //}

    //    //private void TableDependency_Changed(object sender, RecordChangedEventArgs<PostedTrack> e)
    //    //{
    //    //    this.OnPostedTracksChanged?.Invoke(this, new PostedTracksChangeEventArgs(e.Entity, e.EntityOldValues));
    //    //}

    //    public IList<PostedTrack> GetPostedTracks()
    //    {
    //        var result = new List<PostedTrack>();

    //        using (var sqlConnection = new SqlConnection(_configuration["ConnectionString"]))
    //        {
    //            sqlConnection.Open();

    //            using (var command = sqlConnection.CreateCommand())
    //            {
    //                command.CommandText = "SELECT * FROM " + TableName;
    //                command.CommandType = CommandType.Text;

    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    if (reader.HasRows)
    //                    {
    //                        while (reader.Read())
    //                        {
    //                            result.Add(new PostedTrack
    //                            {
    //                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
    //                                MediaId = reader.GetInt32(reader.GetOrdinal("MediaId")),
    //                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
    //                                PlaylistId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
    //                                Trackname = reader.GetString(reader.GetOrdinal("Trackname")),
    //                                Date = reader.GetDateTime(reader.GetOrdinal("Date"))
    //                            });
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        return result;
    //    }


    //    public void Dispose()
    //    {
    //        //_notifier.Stop();
    //        //_notifier.Dispose();
    //    }
    //}
}
