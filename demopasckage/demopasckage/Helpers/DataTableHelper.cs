using System.Data;
using System.Reflection;

namespace Tao_Xuat_Report_Chap9.Helpers;

public static class DataTableHelper
{
    /// <summary>
    /// Chuyển danh sách đối tượng (List&lt;T&gt;) thành DataTable bằng reflection.
    /// Dùng để minh họa nguồn dữ liệu DataTable của FastReport và xuất Excel nhanh.
    /// </summary>
    public static DataTable ToDataTable<T>(IEnumerable<T> nguon, string tenBang)
    {
        var dt = new DataTable(tenBang);

        var props = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0)
            .ToArray();

        foreach (var p in props)
        {
            // DataColumn không nhận kiểu Nullable<T> -> lấy kiểu gốc
            var kieu = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            dt.Columns.Add(p.Name, kieu);
        }

        foreach (var item in nguon)
        {
            var row = dt.NewRow();
            foreach (var p in props)
                row[p.Name] = p.GetValue(item) ?? DBNull.Value;
            dt.Rows.Add(row);
        }

        return dt;
    }
}
