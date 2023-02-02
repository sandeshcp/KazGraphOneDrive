using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using KazGraph.DataServices;
using System.Data;
using System.Data.SqlClient;
using Dapper;


namespace KazGraph.DataServices
{

    public  class DataService 
    {
        public  void ReturnFileListfromDB(ref List<GetFileList> fileList1)
        {

            GetFileList item1 = new GetFileList();
            item1.FileListID = new Guid("CAEAAACC-9485-46EB-BB94-698F204991F4");
            item1.FileImportID = new Guid("38F4803C-9E45-4BD2-801F-8CDB4C1B167B");
            item1.FileName = "Appendix D Supplier Form User Groups.docx";
            item1.FileSizeByte = 124156;
            item1.FilePath = @"https://crmdynamics1.sharepoint.com/sites/SeaspanShipManagement/_layouts/15/Doc.aspx?sourcedoc=%7BDF3A3265-8441-4D7D-B3FC-38591448836D%7D&file=Appendix%20D%20Supplier%20Form%20User%20Groups.docx&action=default&mobileredirect=true";
            //item1.FileCreatedDate = new DateTime("2021-09-02 01:34:15.000");
            //item1.FileLastModifiedDate = new DateTime("2021-09-02 01:34:15.000");
            //item1.FileListIDParent = new Guid("E6F1CDF1-647F-4E43-AEDE-CD26C3E17C13");
            item1.IsFolder = false;
            //item1.CreatedDate = new DateTime("2022-04-01 17:08:03.620");

            fileList1.Add(item1);

            item1 = new GetFileList();
            item1.FileListID = new Guid("CAEAAACC-9485-46EB-BB94-698F204991F5");
            item1.FileImportID = new Guid("38F4803C-9E45-4BD2-801F-8CDB4C1B167B");
            item1.FileName = "test child item 1";
            item1.FileSizeByte = 124156;
            item1.FilePath = @"test child item";
            //item1.FileCreatedDate = new DateTime("2021-09-02 01:34:15.000");
            //item1.FileLastModifiedDate = new DateTime("2021-09-02 01:34:15.000");
            item1.FileListIDParent = new Guid("CAEAAACC-9485-46EB-BB94-698F204991F4");
            item1.IsFolder = false;
            //item1.CreatedDate = new DateTime("2022-04-01 17:08:03.620");

            fileList1.Add(item1);

            item1 = new GetFileList();
            item1.FileListID = new Guid("CAEAAACC-9485-46EB-BB94-698F204991F6");
            item1.FileImportID = new Guid("38F4803C-9E45-4BD2-801F-8CDB4C1B167B");
            item1.FileName = "test child item 2";
            item1.FileSizeByte = 124156;
            item1.FilePath = @"test child item 2";
            //item1.FileCreatedDate = new DateTime("2021-09-02 01:34:15.000");
            //item1.FileLastModifiedDate = new DateTime("2021-09-02 01:34:15.000");
            item1.FileListIDParent = new Guid("CAEAAACC-9485-46EB-BB94-698F204991F4");
            item1.IsFolder = false;
            //item1.CreatedDate = new DateTime("2022-04-01 17:08:03.620");

            fileList1.Add(item1);


            

            using (SqlConnection connection1 = new SqlConnection("Server=CRM345;Database=Kazoo;Trusted_Connection=True;"))
            using (SqlCommand cmd = new SqlCommand(@"DECLARE @latestFileImportID uniqueidentifier = (SELECT TOP 1 FileImportID FROM dbo.FileImport WHERE EndTime IS NOT NULL ORDER BY EndTime DESC)

                                    SELECT
                                        [FileListID]
                                       ,[FileImportID]
                                       ,[FileName]
                                       ,[FileSizeByte]
                                       ,[FilePath]
                                       ,[FileCreatedDate]
                                       ,[FileLastModifiedDate]
                                       ,[FileListIDParent]
                                       ,[IsFolder]
            FROM[dbo].[FileList]
                                        WHERE[FileImportID] = @latestFileImportID
                                        ORDER BY[FileName]", connection1))

            {
                connection1.Open();
                using (SqlDataReader reader1 = cmd.ExecuteReader())
                {
                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            item1 = new GetFileList();
                            item1.FileListID = reader1.GetGuid(reader1.GetOrdinal("FileListID"));
                            item1.FileName = reader1.GetString(reader1.GetOrdinal("FileName"));
                            fileList1.Add(item1);
                        }
                    }
                }

            }


        }
        //public async Task<Guid> SaveAsync(SaveFileList saveFileDetail)
        //{
        //    saveFileDetail.FileListID = Guid.NewGuid();

        //    using var db = GetDbConnection();

        //    await db.ExecuteAsync(@"INSERT INTO [dbo].[FileList]
        //                           ([FileListID]
        //                           ,[FileImportID]
        //                           ,[FileName]
        //                           ,[FileSizeByte]
        //                           ,[FilePath]
        //                           ,[FileCreatedDate]
        //                           ,[FileLastModifiedDate]
        //                           ,[FileListIDParent]
        //                           ,[IsFolder]
        //                           ,[CreatedDate])
        //                            VALUES (@FileListID,@FileImportID,@FileName,@FileSizeByte,@FilePath,@FileCreatedDate,@FileLastModifiedDate,@FileListIDParent,@IsFolder,GetDate())", saveFileDetail);

        //    return saveFileDetail.FileListID;
        //}

        //public async Task<IEnumerable<GetFileList>> GetLatestAsync()
        //{
        //    IDbConnection GetDbConnection() => new SqlConnection("Server=CRM345;Database=Kazoo;Trusted_Connection=True;");
        //    System.Data.IDbConnection db = GetDbConnection();


        //    return await db.QueryAsync<GetFileList>(@"
        //                        DECLARE @latestFileImportID uniqueidentifier = (SELECT TOP 1 FileImportID FROM dbo.FileImport WHERE EndTime IS NOT NULL ORDER BY EndTime DESC)

        //                        SELECT 
        //                            [FileListID]
        //                           ,[FileImportID]
        //                           ,[FileName]
        //                           ,[FileSizeByte]
        //                           ,[FilePath]
        //                           ,[FileCreatedDate]
        //                           ,[FileLastModifiedDate]
        //                           ,[FileListIDParent]
        //                           ,[IsFolder] 
        //                            FROM [dbo].[FileList] 
        //                            WHERE [FileImportID] = @latestFileImportID
        //                            ORDER BY [FileName]");




        //}

        //public async Task<Guid> StartFileImport()
        //{
        //    var fileImportId = Guid.NewGuid();
        //    var startTime = DateTime.UtcNow;

        //    using var db = GetDbConnection();
        //    await db.ExecuteAsync(@"INSERT INTO [dbo].[FileImport]
        //                           ([FileImportID]
        //                           ,[StartTime])
        //                            VALUES (@FileImportID,@StartTime)", new { @FileImportID = fileImportId, @StartTime = startTime });

        //    return fileImportId;
        //}

        //public async Task EndFileImport(Guid fileImportId)
        //{
        //    var endTime = DateTime.UtcNow;

        //    using var db = GetDbConnection();
        //    await db.ExecuteAsync(@"UPDATE [dbo].[FileImport]
        //                            SET EndTime = @EndTime
        //                            WHERE FileImportID = @FileImportID", new { @FileImportID = fileImportId, @EndTime = endTime });
        //}

        //public async Task<DateTime?> GetLastRefreshDate()
        //{
        //    using var db = GetDbConnection();

        //    return await db.QueryFirstOrDefaultAsync<DateTime?>(@"
        //                        DECLARE @lastRefreshDate datetime = (SELECT TOP 1 EndTime FROM dbo.FileImport WHERE EndTime IS NOT NULL ORDER BY EndTime DESC)
        //                        SELECT @lastRefreshDate");

        //}



    }
}