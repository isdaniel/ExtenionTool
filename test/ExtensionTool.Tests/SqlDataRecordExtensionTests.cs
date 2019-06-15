using NUnit.Framework;
using ExtensionTool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;


namespace ExtensionTool.Tests
{
    [TestFixture()]
    public class SqlDataRecordExtensionTests
    {
        [Test()]
        public void Get_UDT_ThreeData()
        {
            List<Student> list = new List<Student>()
            {
                new Student
                {
                    Name = "A",
                    No = 1,
                    Class = "111",
                    IDNo = "A123456789"
                },
                new Student
                {
                    Name = "B",
                    No = 2,
                    Class = "222",
                    IDNo = "B123456789"
                },
                new Student
                {
                    Name = "C",
                    No = 3,
                    Class = "333",
                    IDNo = "C123456789"
                }
            };

            var records = list.GetDataRecords();
    
            Assert.AreEqual(3, records.Count());
            Assert.AreEqual(7, records.FirstOrDefault().FieldCount);
        }
        [Test()]
        public void Data_IsEmpty_CountZero()
        {
            List<Student> model = new List<Student>();

            int expectCount = 0;

            var records = model.GetDataRecords();

            Assert.AreEqual(expectCount, records.Count());
        }
    }

    public class Student
    {
        [MetaData(Length = 20, DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }
        [MetaData(DbType = SqlDbType.Int)]
        public int No { get; set; }
        [MetaData(Length = 20, DbType = SqlDbType.VarChar)]
        public string IDNo { get; set; }
        [MetaData(Length = 100, DbType = SqlDbType.NVarChar)]
        public string Address { get; set; }
        [MetaData(Length = 50, DbType = SqlDbType.VarChar)]
        public string Email { get; set; }
        [MetaData(Length = 20, DbType = SqlDbType.VarChar)]
        public string Phone { get; set; }
        [MetaData(Length = 5, DbType = SqlDbType.VarChar)]
        public string Class { get; set; }
    }
}