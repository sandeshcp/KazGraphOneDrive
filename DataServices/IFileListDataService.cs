using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace KazGraph.DataServices
{

        public interface IFileListDataService
        {
            //Task<Guid> SaveAsync(SaveFileList saveFileDetail);
            Task<IEnumerable<GetFileList>> GetLatestAsync();
            //Task<Guid> StartFileImport();
            //Task EndFileImport(Guid fileImportId);
            //Task<DateTime?> GetLastRefreshDate();
        }

}