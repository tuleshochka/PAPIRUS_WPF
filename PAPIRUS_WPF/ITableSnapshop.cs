using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PAPIRUS_WPF
{
    public interface ITableSnapshot : IEnumerable<RowId>
    {
        //StoreSnapshot StoreSnapshot { get; }
        string Name { get; }
        IEnumerable<RowId> Rows { get; }
        bool WasChanged(int fromVersion, int toVersion);
        bool WasAffected(int version);
    }
}
