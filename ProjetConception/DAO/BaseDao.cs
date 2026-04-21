namespace ProjetConception.DAO;
using Microsoft.Data.SqlClient;
using ProjetConception.Models;
using System.Threading.Tasks;

public abstract class BaseDao
{
    private readonly string _connectionString =
        "Server=invl99-1.fortiddns.com,3333;Database=inf1008;User Id=dbuser;Password=XXga4Rn9Hrj*9iWmLRRTSnkRxW9p@xAZ;TrustServerCertificate=True;";

    protected SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}