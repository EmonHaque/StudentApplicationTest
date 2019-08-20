using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InsertExistingPartner(1, 10);
            InSertExistingManager(11, 1, 10, 5);
            InsertExPartner(61, 1000);
            InSertExEmployee(1001, 2000);
            InsertArticled(2001, 100);
            InsertAFR(7001, 100);
            InsertApprentice(12001, 100);
            InsertCC(17001, 100);
            InsertExApprentice(22001, 100);
            InsertExAFR(27001, 100);
            InsertExArticled(32001, 100);
            InsertExCC(37001, 100);
            InsertApplicantForArticleship(42001, 45000);
            InsertApplicantForJob(45001, 50000);
            InsertJoiningLetters(50001, 100);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

        }

        static void InsertJoiningLetters(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Joining'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Joined";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Name = name + " " + i;
                            scv.Applications.Joining.Id = scv.Id;
                            scv.Applications.Joining.PartnerId = pm.PartnerId;
                            //scv.Applications.Joining.
                            ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4NewJoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Joining"), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " Joined Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total +" Joining Letter Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertApplicantForArticleship(int startIndex, int endIndex)
        {
            int Id = 0;
            int total = endIndex - startIndex + 1;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Applicant for Articleship'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";

            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Applicant for Articleship";
                    for (int i = startIndex; i < endIndex + 1; i++)
                    {
                        scv.Id = startIndex;
                        scv.Name = name + " " + i;
                        ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4Application, Para4NewApplicants(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();

                        foreach (var item in scv.Education)
                        {
                            ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.SubjectsOA)
                        {
                            ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.Reference)
                        {
                            ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }

                        var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                        var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                        File.Copy(from, to);
                        //Console.WriteLine(startIndex + " Applicant for Articleship");
                        startIndex++;
                    }


                    Trans.Commit();
                    Console.WriteLine(total + " Applicant for Articleship Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertApplicantForJob(int startIndex, int endIndex)
        {
            int Id = 0;
            int total = endIndex - startIndex + 1;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Applicant for Job'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";

            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Applicant for Job";
                    for (int i = startIndex; i < endIndex + 1; i++)
                    {
                        scv.Id = startIndex;
                        scv.Name = name + " " + i;
                        ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4Application, Para4NewApplicants(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();

                        foreach (var item in scv.Education)
                        {
                            ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.SubjectsOA)
                        {
                            ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.Reference)
                        {
                            ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }

                        var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                        var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                        File.Copy(from, to);
                        //Console.WriteLine(startIndex + " Applicant for Job");
                        startIndex++;
                    }


                    Trans.Commit();
                    Console.WriteLine(total +" Applicant for Job Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertExApprentice(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Apprentice'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            //string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Ex Apprentice";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Name = name + " " + i;
                            scv.LeftOn = DateTime.Now.Date;
                            scv.Status = Convert.ToString(17); // 17 for left 15 for terminated
                            scv.SupervisingHistory.Last().To = scv.LeftOn;
                            ExecuteTransaction(q4Student, Para4Student(scv, true,13), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            //ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " Ex Apprentice Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " Ex Apprentice Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertApprentice(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;

            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Apprentice'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Apprentice";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Name = name + " " + i;
                            ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " Apprentice Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " Apprentice Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertExAFR(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Applied for Registration'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            rg.PeriodOfCourse = rd["Period"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Period"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4Registration = "INSERT INTO StudentRegistration VALUES(@Id, @Date, @Period)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Ex Applied for Registration";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Name = name + " " + i;
                            scv.LeftOn = DateTime.Now.Date;
                            scv.Status = Convert.ToString(15); //17 for left 15 for terminated
                            scv.SupervisingHistory.Last().To = scv.LeftOn;
                            ExecuteTransaction(q4Student, Para4Student(scv, true,14), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            //ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Registration, Para4Registration(scv), con, Trans).ExecuteNonQuery();
                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + "Ex AFR Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " Ex AFR Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertAFR(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Applied for Registration'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            rg.PeriodOfCourse = rd["Period"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Period"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";
            string q4Registration = "INSERT INTO StudentRegistration VALUES(@Id, @Date, @Period)";
            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Applied for Registration";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Name = name + " " + i;
                            ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Registration, Para4Registration(scv), con, Trans).ExecuteNonQuery();
                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " AFR Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " AFR Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertExArticled(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Articled'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            rg.PeriodOfCourse = rd["Period"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Period"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4Registration = "INSERT INTO StudentRegistration VALUES(@Id, @Date, @Period)";
            //string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Ex Articled";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Registration.Id = scv.Id;
                            scv.Name = name + " " + i;
                            scv.Status = Convert.ToString(17); //17 for left 15 for terminated
                            scv.LeftOn = DateTime.Now.Date;
                            scv.SupervisingHistory.Last().To = scv.LeftOn;
                            ExecuteTransaction(q4Student, Para4Student(scv, true,6), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Registration, Para4Registration(scv), con, Trans).ExecuteNonQuery();
                            //ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " Ex Articled Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " Ex Articled Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertArticled(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;

            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Articled'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            rg.PeriodOfCourse = rd["Period"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Period"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";
            string q4Registration = "INSERT INTO StudentRegistration VALUES(@Id, @Date, @Period)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Articled";
                    Random rand = new Random(DateTime.Now.Millisecond);
                    var regDate = scv.Registration.Date;
                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Registration.Date = regDate.AddYears(rand.Next(-3, 2));
                            
                            scv.Id = startIndex;
                            scv.Registration.Id = scv.Id;
                            scv.Name = name + " " + i;
                            ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Registration, Para4Registration(scv), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " Articled Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " Articled Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertExCC(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Course Complete'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            rg.PeriodOfCourse = rd["Period"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Period"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            //string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";
            string q4Registration = "INSERT INTO StudentRegistration VALUES(@Id, @Date, @Period)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Ex Course Complete";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Registration.Id = scv.Id;
                            scv.Name = name + " " + i;
                            scv.LeftOn = DateTime.Now.Date;
                            scv.Status = Convert.ToString(17);//17 for left 15 for terminated
                            scv.SupervisingHistory.Last().To = scv.LeftOn;

                            ExecuteTransaction(q4Student, Para4Student(scv, true,16), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            //ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Registration, Para4Registration(scv), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = 0;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " Ex CC Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " Ex CC Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertCC(int startIndex, int howMany)
        {
            List<PartManInfo> partMan = new List<PartManInfo>();
            #region Extrat Partner and Manager Id

            string query = "SELECT Id, PartnerId From HRSupervisory WHERE StatusId = 4 AND [To] IS Null";
            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        PartManInfo pmi = new PartManInfo();
                        pmi.ManagerId = Convert.ToInt32(rd["Id"]);
                        pmi.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                        partMan.Add(pmi);
                    }
                }
            }
            #endregion

            int Id = 0;
            int totalPartner = partMan.Select(x => x.PartnerId).Distinct().Count();
            int managerPerPartner = partMan.Count() / totalPartner;
            int total = totalPartner * managerPerPartner * howMany;
            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.Reference = new List<ReferenceInfo>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Course Complete'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id;
                                     SELECT * FROM StudentRegistration";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();
                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Registration rg = new Registration();
                            rg.Id = Convert.ToInt32(rd["Id"]);
                            rg.Date = Convert.ToDateTime(rd["Date"]);
                            rg.PeriodOfCourse = rd["Period"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Period"]);
                            scv.Registration = rg;
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";
            string q4Registration = "INSERT INTO StudentRegistration VALUES(@Id, @Date, @Period)";

            //string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Course Complete";

                    foreach (var pm in partMan)
                    {
                        for (int i = 1; i < howMany + 1; i++)
                        {
                            scv.Id = startIndex;
                            scv.Name = name + " " + i;
                            scv.Registration.Id = scv.Id;
                            ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Student"), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Registration, Para4Registration(scv), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = pm.ManagerId;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = pm.PartnerId;
                                item.ManagerId = 0;
                                //item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }

                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startIndex + " CC Inserted");
                            startIndex++;
                        }
                    }

                    Trans.Commit();
                    Console.WriteLine(total + " CC Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

        }

        static void InsertExistingPartner(int startIndex, int endIndex)
        {
            int Id = 0;

            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.CurrentJob = new CurrentPosition();
            scv.Experiences = new List<Experience>();
            scv.Reference = new List<ReferenceInfo>();
            scv.PartnerMembership = new List<PartnerMembership>();
            scv.PartnerCurrentPosition = new List<PartnerCurrentPosition>();
            scv.PartnerPastPosition = new List<PartnerPastPosition>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Ex Partner'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM CurrentPosition WHERE Id = @Id; 
                                     SELECT * FROM PastPosition WHERE Id = @Id;
                                     SELECT * FROM PartnerMembership WHERE Id = @Id;
                                     SELECT * FROM PartnerCurrentPosition WHERE Id = @Id;
                                     SELECT * FROM PartnerPastPosition WHERE Id = @Id";

            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = Convert.ToDateTime("1/1/1905");
                        //scv.LeftOn = rd["LeftOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["LeftOn"]);
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.CurrentJob.Position = rd["Position"].ToString();
                            scv.CurrentJob.Organisation = rd["Organisation"].ToString();
                            scv.CurrentJob.From = Convert.ToDateTime(rd["From"]);
                            scv.CurrentJob.Responsibilities = rd["Responsibilities"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Experience ep = new Experience();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            ep.From = Convert.ToDateTime(rd["To"]);
                            ep.Responsibilities = rd["Responsibilities"].ToString();
                            scv.Experiences.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            PartnerMembership ep = new PartnerMembership();
                            ep.Id = rd["MemberId"].ToString();
                            ep.Organisation = rd["OrgId"].ToString();
                            scv.PartnerMembership.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            PartnerCurrentPosition ep = new PartnerCurrentPosition();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            scv.PartnerCurrentPosition.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            PartnerPastPosition ep = new PartnerPastPosition();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            ep.To = Convert.ToDateTime(rd["To"]);
                            scv.PartnerPastPosition.Add(ep);
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4PartMembership = "INSERT INTO PartnerMembership VALUES(@Id, @MemberId, @OrgId)";
            string q4PartCurrentPos = "INSERT INTO PartnerCurrentPosition VALUES(@Id, @Position, @Organisation, @From)";
            string q4PartPastPos = "INSERT INTO PartnerPastPosition VALUES(@Id, @Position, @Organisation, @From, @To)";
            string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";

            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Existing Partner";
                    for (int i = startIndex; i < endIndex + 1; i++)
                    {

                        scv.Id = i;
                        scv.Name = name + " " + i;
                        ExecuteTransaction(q4Student, Para4Student(scv, true,0), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Partner"), con, Trans).ExecuteNonQuery();

                        foreach (var item in scv.Education)
                        {
                            ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.PartnerMembership)
                        {
                            ExecuteTransaction(q4PartMembership, Para4PartMembership(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.PartnerCurrentPosition)
                        {
                            ExecuteTransaction(q4PartCurrentPos, Para4PartCurrentPosition(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.PartnerPastPosition)
                        {
                            ExecuteTransaction(q4PartPastPos, Para4PartPastPosition(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.Reference)
                        {
                            ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                        var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                        File.Copy(from, to);
                        //Console.WriteLine(i + " Existing Partner Inserted");
                    }
                    Trans.Commit();
                    Console.WriteLine(endIndex-startIndex+1 +" Existing Partner Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion
        }

        static void InSertExistingManager(int startingId, int startPartnerIndex, int endPartnerIndex, int noOfManagersPerPartner)
        {
            int total = noOfManagersPerPartner * (endPartnerIndex - startPartnerIndex + 1);
            int Id = 0;

            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.CurrentJob = new CurrentPosition();
            scv.Experiences = new List<Experience>();
            scv.Reference = new List<ReferenceInfo>();
            scv.PartnerMembership = new List<PartnerMembership>();
            scv.PartnerCurrentPosition = new List<PartnerCurrentPosition>();
            scv.PartnerPastPosition = new List<PartnerPastPosition>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Ex Employee'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM CurrentPosition WHERE Id = @Id; 
                                     SELECT * FROM PastPosition WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = rd["LeftOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["LeftOn"]);
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.CurrentJob.Position = rd["Position"].ToString();
                            scv.CurrentJob.Organisation = rd["Organisation"].ToString();
                            scv.CurrentJob.From = Convert.ToDateTime(rd["From"]);
                            scv.CurrentJob.Responsibilities = rd["Responsibilities"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Experience ep = new Experience();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            ep.To = Convert.ToDateTime(rd["To"]);
                            ep.Responsibilities = rd["Responsibilities"].ToString();
                            scv.Experiences.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();

                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                }
            }
            #endregion

            #region Insert CV

            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4CurrentJob = "INSERT INTO CurrentPosition VALUES(@Id, @Position, @Organisation, @From, @Responsibilities)";
            string q4PastJobs = "INSERT INTO PastPosition VALUES(@Id, @Position, @Organisation, @From, @To, @Responsibilities)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";
            string q4UserTable = "INSERT INTO UserTable VALUES(@StudentId, @Name, @Password, @Role, @Retry, @Status)";

            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = "Manger";
                    scv.Status = Convert.ToString(4); // 4 is for Manager
                    scv.LeftOn = Convert.ToDateTime("1/1/1905");

                    for (int i = startPartnerIndex; i < endPartnerIndex + 1; i++)
                    {
                        for (int j = 1; j < noOfManagersPerPartner + 1; j++)
                        {
                            scv.Id = startingId;
                            scv.Name = name + " " + startingId;
                            scv.Applications.Joining.PartnerId = i;

                            ExecuteTransaction(q4Student, Para4Student(scv, false,0), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4CurrentJob, Para4CurrentPosition(scv.Id, scv.CurrentJob), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();
                            ExecuteTransaction(q4UserTable, Para4UserTable(scv.Id, "Manager"), con, Trans).ExecuteNonQuery();

                            foreach (var item in scv.Education)
                            {
                                ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SubjectsOA)
                            {
                                ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Experiences)
                            {
                                ExecuteTransaction(q4PastJobs, Para4PastPosition(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Applications.Applications)
                            {
                                item.PartnerId = i;
                                ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.SupervisingHistory)
                            {
                                item.PartnerId = i;
                                item.To = Convert.ToDateTime("1/1/1905");
                                ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            foreach (var item in scv.Reference)
                            {
                                ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                            }
                            var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                            var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                            File.Copy(from, to);
                            //Console.WriteLine(startingId + " Manager Inserted");
                            startingId++;
                        }

                    }
                    Trans.Commit();
                    Console.WriteLine(total + " Manager Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion


        }

        static void InsertExPartner(int startIndex, int endIndex)
        {
            int Id = 0;

            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.CurrentJob = new CurrentPosition();
            scv.Experiences = new List<Experience>();
            scv.Reference = new List<ReferenceInfo>();
            scv.PartnerMembership = new List<PartnerMembership>();
            scv.PartnerCurrentPosition = new List<PartnerCurrentPosition>();
            scv.PartnerPastPosition = new List<PartnerPastPosition>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Ex Partner'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM CurrentPosition WHERE Id = @Id; 
                                     SELECT * FROM PastPosition WHERE Id = @Id;
                                     SELECT * FROM PartnerMembership WHERE Id = @Id;
                                     SELECT * FROM PartnerCurrentPosition WHERE Id = @Id;
                                     SELECT * FROM PartnerPastPosition WHERE Id = @Id";

            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = rd["LeftOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["LeftOn"]);
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.CurrentJob.Position = rd["Position"].ToString();
                            scv.CurrentJob.Organisation = rd["Organisation"].ToString();
                            scv.CurrentJob.From = Convert.ToDateTime(rd["From"]);
                            scv.CurrentJob.Responsibilities = rd["Responsibilities"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Experience ep = new Experience();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            ep.From = Convert.ToDateTime(rd["To"]);
                            ep.Responsibilities = rd["Responsibilities"].ToString();
                            scv.Experiences.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            PartnerMembership ep = new PartnerMembership();
                            ep.Id = rd["MemberId"].ToString();
                            ep.Organisation = rd["OrgId"].ToString();
                            scv.PartnerMembership.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            PartnerCurrentPosition ep = new PartnerCurrentPosition();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            scv.PartnerCurrentPosition.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            PartnerPastPosition ep = new PartnerPastPosition();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            ep.To = Convert.ToDateTime(rd["To"]);
                            scv.PartnerPastPosition.Add(ep);
                        }
                    }
                }
            }
            #endregion

            #region Insert CV
            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4PartMembership = "INSERT INTO PartnerMembership VALUES(@Id, @MemberId, @OrgId)";
            string q4PartCurrentPos = "INSERT INTO PartnerCurrentPosition VALUES(@Id, @Position, @Organisation, @From)";
            string q4PartPastPos = "INSERT INTO PartnerPastPosition VALUES(@Id, @Position, @Organisation, @From, @To)";

            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = scv.Name;
                    for (int i = startIndex; i < endIndex + 1; i++)
                    {

                        scv.Id = i;
                        scv.Name = name + " " + i;
                        ExecuteTransaction(q4Student, Para4Student(scv, false,0), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();

                        foreach (var item in scv.Education)
                        {
                            ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.PartnerMembership)
                        {
                            ExecuteTransaction(q4PartMembership, Para4PartMembership(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.PartnerCurrentPosition)
                        {
                            ExecuteTransaction(q4PartCurrentPos, Para4PartCurrentPosition(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.PartnerPastPosition)
                        {
                            ExecuteTransaction(q4PartPastPos, Para4PartPastPosition(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.Reference)
                        {
                            ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                        var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                        File.Copy(from, to);
                        //Console.WriteLine(i + " Ex Partner Inserted");
                    }
                    Trans.Commit();
                    Console.WriteLine(endIndex - startIndex + 1 + " Ex Partner Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion
        }

        static void InSertExEmployee(int startIndex, int endIndex)
        {
            int Id = 0;

            StudentCV scv = new StudentCV();
            scv.Education = new List<UniversityInfo>();
            scv.SubjectsOA = new List<SubjectsOfOA>();
            scv.CurrentJob = new CurrentPosition();
            scv.Experiences = new List<Experience>();
            scv.Reference = new List<ReferenceInfo>();
            scv.PartnerMembership = new List<PartnerMembership>();
            scv.PartnerCurrentPosition = new List<PartnerCurrentPosition>();
            scv.PartnerPastPosition = new List<PartnerPastPosition>();
            scv.Applications = new StudentApplications();
            scv.Applications.Applications = new List<RegularApplication>();
            scv.SupervisingHistory = new List<HRSupervisory>();

            #region Populate CV
            string query4Id = "SELECT Id FROM Student WHERE Name = 'Ex Employee'";
            string query4Extract = @"SELECT * FROM Student WHERE Id = @Id;
                                     SELECT * FROM StudentDetail WHERE Id = @Id;
                                     SELECT * From StudentEducation WHERE Id = @Id;
                                     SELECT * FROM StudentOAEducation WHERE Id = @Id;
                                     SELECT * FROM Reference WHERE Id = @Id;
                                     SELECT * FROM CurrentPosition WHERE Id = @Id; 
                                     SELECT * FROM PastPosition WHERE Id = @Id;
                                     SELECT * FROM NewApplication WHERE Id = @Id;
                                     SELECT * FROM JoiningLetter WHERE Id = @Id;
                                     SELECT * FROM StudentApplication WHERE StudentId = @Id;
                                     SELECT * FROM HRSupervisory WHERE Id = @Id";
            string cs1 = ConfigurationManager.ConnectionStrings["StudentDB4Test"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs1))
            {
                using (SqlCommand cmd = new SqlCommand(query4Id, con))
                {
                    con.Open();
                    Id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
                using (SqlCommand cmd = new SqlCommand(query4Extract, con))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        scv.Id = Convert.ToInt32(rd["Id"]);
                        scv.Name = rd["Name"].ToString();
                        scv.RegistrationNo = rd["RegistrationNo"] == DBNull.Value ? null : rd["RegistrationNo"].ToString();
                        scv.Status = rd["Status"] == DBNull.Value ? null : rd["Status"].ToString();
                        scv.AppliedOn = rd["AppliedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["AppliedOn"]);
                        scv.JoinedOn = rd["JoinedOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["JoinedOn"]);
                        scv.LeftOn = rd["LeftOn"] == DBNull.Value ? Convert.ToDateTime("01/01/1905") : Convert.ToDateTime(rd["LeftOn"]);
                        scv.DateOfBirth = Convert.ToDateTime(rd["DoB"]);
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.FatherName = rd["FatherName"].ToString();
                            scv.MotherName = rd["MotherName"].ToString();
                            scv.Religion = rd["Religion"].ToString();
                            scv.Gender = Convert.ToBoolean(rd["Gender"]) == true ? "Male" : "Female";
                            scv.MaritalStatus = rd["MaritalStatus"].ToString();
                            scv.Nationality = rd["Nationality"].ToString();
                            scv.Blood = rd["Blood"].ToString();
                            scv.ContactNo = rd["ContactNo"].ToString();
                            scv.Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString();
                            scv.PresentAddress = rd["PresentAddress"].ToString();
                            scv.PermanentAddress = rd["PermanentAddress"].ToString();
                            scv.EmergencyContactNo = rd["EmergencyContactNo"].ToString();
                            scv.RelationWithEmergencyContact = rd["RelationWithEmergencyContact"].ToString();
                            scv.Objective = rd["Objective"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            UniversityInfo ui = new UniversityInfo();
                            ui.Title = rd["Title"].ToString();
                            ui.Type = rd["Type"] == DBNull.Value ? null : rd["Type"].ToString();
                            ui.University = rd["BoardUniversity"].ToString();
                            ui.Institute = rd["Institute"].ToString();
                            ui.YearOfPass = rd["YearOfPass"].ToString();
                            ui.CGPA = rd["CGPA"].ToString();
                            ui.Major = rd["Major"] == DBNull.Value ? null : rd["Major"].ToString();
                            scv.Education.Add(ui);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            SubjectsOfOA soa = new SubjectsOfOA();
                            soa.Level = rd["Level"].ToString();
                            soa.Name = rd["Subject"].ToString();
                            soa.Grade = rd["Grade"].ToString();
                            scv.SubjectsOA.Add(soa);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            ReferenceInfo reference = new ReferenceInfo();
                            reference.Name = rd["Name"].ToString();
                            reference.Designation = rd["Designation"].ToString();
                            reference.Organisation = rd["Organisation"].ToString();
                            reference.ContactNo = rd["ContactNo"].ToString();
                            reference.Relation = rd["Relation"].ToString();
                            scv.Reference.Add(reference);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            scv.CurrentJob.Position = rd["Position"].ToString();
                            scv.CurrentJob.Organisation = rd["Organisation"].ToString();
                            scv.CurrentJob.From = Convert.ToDateTime(rd["From"]);
                            scv.CurrentJob.Responsibilities = rd["Responsibilities"].ToString();
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            Experience ep = new Experience();
                            ep.Position = rd["Position"].ToString();
                            ep.Organisation = rd["Organisation"].ToString();
                            ep.From = Convert.ToDateTime(rd["From"]);
                            ep.To = Convert.ToDateTime(rd["To"]);
                            ep.Responsibilities = rd["Responsibilities"].ToString();
                            scv.Experiences.Add(ep);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            NewApplication newApp = new NewApplication();
                            newApp.AppSubject = rd["AppSubject"].ToString();
                            newApp.AppBody = rd["AppBody"].ToString();
                            newApp.StatusId = Convert.ToInt32(rd["StatusId"]);
                            scv.Applications.FirstApplication = newApp;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            JoiningLetter let = new JoiningLetter();
                            let.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            let.AppSubject = rd["AppSubject"].ToString();
                            let.AppBody = rd["AppBody"].ToString();
                            let.Date = Convert.ToDateTime(rd["Date"]);
                            scv.Applications.Joining = let;
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            RegularApplication regApp = new RegularApplication();

                            regApp.Sl = Convert.ToInt32(rd["Sl"]);
                            regApp.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["PartnerId"]);
                            regApp.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            regApp.AppTypeId = Convert.ToInt32(rd["ApplicationTypeId"]);
                            regApp.AppBody = rd["ApplicationBody"].ToString();
                            regApp.Date = Convert.ToDateTime(rd["Date"]);
                            regApp.ManagerNote = rd["ManagerNote"] == DBNull.Value ? null : rd["ManagerNote"].ToString();
                            regApp.PartnerNote = rd["PartnerNote"] == DBNull.Value ? null : rd["PartnerNote"].ToString();
                            regApp.StudentReview = Convert.ToBoolean(rd["StudentReview"]);
                            regApp.ManagerReview = Convert.ToBoolean(rd["ManagerReview"]);
                            regApp.PartnerReview = Convert.ToBoolean(rd["PartnerReview"]);
                            if (rd["Accepted"] == DBNull.Value)
                            {
                                regApp.Accepted = null;
                            }
                            else
                            {
                                regApp.Accepted = Convert.ToBoolean(rd["Accepted"]);
                            }
                            scv.Applications.Applications.Add(regApp);
                        }
                    }

                    if (rd.NextResult())
                    {
                        while (rd.Read())
                        {
                            HRSupervisory at = new HRSupervisory();
                            at.Id = Convert.ToInt32(rd["Id"]);
                            at.PartnerId = Convert.ToInt32(rd["PartnerId"]);
                            at.ManagerId = rd["ManagerId"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ManagerId"]);
                            at.DepartmentId = Convert.ToInt32(rd["DepartmentId"]);
                            at.StatusId = Convert.ToInt32(rd["StatusId"]);
                            at.From = Convert.ToDateTime(rd["From"]);
                            at.To = rd["To"] == DBNull.Value ? Convert.ToDateTime("1/1/1905") : Convert.ToDateTime(rd["To"]);
                            scv.SupervisingHistory.Add(at);
                        }
                    }
                }
            }
            #endregion

            #region Insert CV

            string q4Student = "INSERT INTO Student VALUES(@Id, @Name, @RegistrationNo, @Status, @Department, @AppliedOn, @JoinedOn, @LeftOn, @DoB, @SiteUser, @LastStat)";
            string q4StudentDetail = @"INSERT INTO StudentDetail VALUES(@Id, @FatherName, @MotherName, @Religion, @Gender, 
                                                        @MaritalStatus, @Nationality, @Blood, @ContactNo, @Email, @PresentAddress, 
                                                        @PermanentAddress, @EmergencyContactNo, 
                                                        @RelationWithEmergencyContact, @Objective)";
            string q4StudentEducation = @"INSERT INTO StudentEducation VALUES(@Id, @Type, @Title, @BoardUniversity, @Institute, 
                                                        @YearOfPass, @CGPA, @Major)";
            string q4Reference = "INSERT INTO Reference VALUES(@Id, @Name, @Designation, @Organisation, @ContactNo, @Relation)";
            string q4StudentOAEducation = "INSERT INTO StudentOAEducation VALUES(@Id, @Level, @Subject, @Grade)";
            string q4CurrentJob = "INSERT INTO CurrentPosition VALUES(@Id, @Position, @Organisation, @From, @Responsibilities)";
            string q4PastJobs = "INSERT INTO PastPosition VALUES(@Id, @Position, @Organisation, @From, @To, @Responsibilities)";
            string q4Application = "INSERT INTO NewApplication VALUES(@Id, @AppSubject, @AppBody, @StatusId, @Accepted)";
            string q4JoiningLetter = "INSERT INTO JoiningLetter VALUES(@Id, @PartnerId, @AppSubject, @AppBody, @Date, @Reviewed)";
            string q4RegularApplication = @"INSERT INTO StudentApplication VALUES(@StudentId, @ManagerId, @PartnerId, 
                             @ApplicationTypeId, @ApplicationBody, @Date, @ManagerNote, @PartnerNote, 
                             @StudentReview, @ManagerReview, @PartnerReview, @Accepted)";
            string q4HrSupervisory = "INSERT INTO HRSupervisory VALUES(@Id, @PartnerId, @ManagerId, @DepartmentId, @StatusId, @From, @To)";

            string cs2 = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs2))
            {
                con.Open();
                SqlTransaction Trans = con.BeginTransaction();
                try
                {
                    string name = scv.Name;
                    for (int i = startIndex; i < endIndex + 1; i++)
                    {

                        scv.Id = i;
                        scv.Name = name + " " + i;
                        ExecuteTransaction(q4Student, Para4Student(scv, false,4), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4StudentDetail, Para4StudentDetail(scv), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4CurrentJob, Para4CurrentPosition(scv.Id, scv.CurrentJob), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4Application, Para4FirstApplication(scv.Id, scv.Applications.FirstApplication), con, Trans).ExecuteNonQuery();
                        ExecuteTransaction(q4JoiningLetter, Para4JoiningLetter(scv.Id, scv.Applications.Joining), con, Trans).ExecuteNonQuery();

                        foreach (var item in scv.Education)
                        {
                            ExecuteTransaction(q4StudentEducation, Para4Education(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.SubjectsOA)
                        {
                            ExecuteTransaction(q4StudentOAEducation, para4OASubject(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.Experiences)
                        {
                            ExecuteTransaction(q4PastJobs, Para4PastPosition(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.Applications.Applications)
                        {
                            ExecuteTransaction(q4RegularApplication, Para4RegularApps(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.SupervisingHistory)
                        {
                            ExecuteTransaction(q4HrSupervisory, Para4HRSupervisory(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        foreach (var item in scv.Reference)
                        {
                            ExecuteTransaction(q4Reference, Para4Reference(scv.Id, item), con, Trans).ExecuteNonQuery();
                        }
                        var from = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\AKMMH.jpg";
                        var to = @"D:\StudentAppRecovered\StudentAppRecovered\Photo\" + scv.Id + " " + scv.Name + ".jpg";
                        File.Copy(from, to);
                        //Console.WriteLine(i + " Ex Employee Inserted");
                    }
                    Trans.Commit();
                    Console.WriteLine(endIndex - startIndex + 1 + " Ex Employee Transaction Committed");
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion


        }

        public static SqlParameter[] Para4PartCurrentPosition(int id, PartnerCurrentPosition cp)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@Position", cp.Position),
                new SqlParameter("@Organisation", cp.Organisation),
                new SqlParameter("@From", cp.From)
            };
            return para;
        }
        public static SqlParameter[] Para4PartPastPosition(int id, PartnerPastPosition ep)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@Position", ep.Position),
                new SqlParameter("@Organisation", ep.Organisation),
                new SqlParameter("@From", ep.From),
                new SqlParameter("@To", ep.To)
            };
            return para;
        }
        public static SqlParameter[] Para4PartMembership(int id, PartnerMembership ep)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@MemberId", ep.Id),
                new SqlParameter("@OrgId", ep.Organisation)
            };
            return para;
        }
        public static SqlParameter[] Para4CurrentPosition(int id, CurrentPosition cp)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@Position", cp.Position),
                new SqlParameter("@Organisation", cp.Organisation),
                new SqlParameter("@From", cp.From),
                new SqlParameter("@Responsibilities", cp.Responsibilities)
            };
            return para;
        }
        public static SqlParameter[] Para4PastPosition(int id, Experience ep)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@Position", ep.Position),
                new SqlParameter("@Organisation", ep.Organisation),
                new SqlParameter("@From", ep.From),
                new SqlParameter("@To", ep.To),
                new SqlParameter("@Responsibilities", ep.Responsibilities)
            };
            return para;
        }
        public static SqlParameter[] Para4Reference(int id, ReferenceInfo reference)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@Name", reference.Name),
                new SqlParameter("@Designation", reference.Designation),
                new SqlParameter("@Organisation", reference.Organisation),
                new SqlParameter("@ContactNo", reference.ContactNo),
                new SqlParameter("@Relation", reference.Relation)
            };
            return para;
        }
        public static SqlParameter[] Para4Education(int id, UniversityInfo ui)
        {
            SqlParameter major = ui.Major == null ? new SqlParameter("@Major", DBNull.Value) : new SqlParameter("@Major", ui.Major);
            SqlParameter type = ui.Type == null ? new SqlParameter("@Type", DBNull.Value) : new SqlParameter("@Type", ui.Type);
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                type,
                new SqlParameter("@Title", ui.Title),
                new SqlParameter("@BoardUniversity", ui.University),
                new SqlParameter("@Institute", ui.Institute),
                new SqlParameter("@YearOfPass", ui.YearOfPass),
                new SqlParameter("@CGPA", ui.CGPA),
                major
            };
            return para;
        }
        public static SqlParameter[] Para4StudentDetail(StudentCV student)
        {
            SqlParameter Gender = student.Gender == "Male" ? new SqlParameter("@Gender", true) : new SqlParameter("@Gender", false);
            SqlParameter Email = student.Email == null ? new SqlParameter("@Email", DBNull.Value) : new SqlParameter("@Email", student.Email);
            SqlParameter[] para = {
                new SqlParameter("@Id", student.Id),
                new SqlParameter("@FatherName", student.FatherName),
                new SqlParameter("@MotherName", student.MotherName),
                new SqlParameter("@Religion", student.Religion),
                Gender,
                new SqlParameter("@MaritalStatus", student.MaritalStatus),
                new SqlParameter("@Nationality", student.Nationality),
                new SqlParameter("@Blood", student.Blood),
                new SqlParameter("@ContactNo", student.ContactNo),
                Email,
                new SqlParameter("@PresentAddress", student.PresentAddress),
                new SqlParameter("@PermanentAddress", student.PermanentAddress),
                new SqlParameter("@EmergencyContactNo", student.EmergencyContactNo),
                new SqlParameter("@RelationWithEmergencyContact", student.RelationWithEmergencyContact),
                new SqlParameter("@Objective", student.Objective)
            };
            return para;
        }
        public static SqlParameter[] Para4Student(StudentCV student, bool siteUser, int lastStat)
        {
            SqlParameter leftOn = student.LeftOn.Year == 1905 ?
                new SqlParameter("@LeftOn", DBNull.Value) : new SqlParameter("@LeftOn", student.LeftOn);

            SqlParameter joinedOn = student.JoinedOn.Year == 1905 ?
                new SqlParameter("@JoinedOn", DBNull.Value) : new SqlParameter("@JoinedOn", student.JoinedOn);

            SqlParameter registrationNo = student.RegistrationNo == null ?
                new SqlParameter("@RegistrationNo", DBNull.Value) : new SqlParameter("@RegistrationNo", student.RegistrationNo);

            SqlParameter lastStatus = lastStat == 0 ?
                new SqlParameter("@LastStat", DBNull.Value) : new SqlParameter("@LastStat", lastStat);

            SqlParameter[] para = {
                new SqlParameter("@Id", student.Id),
                new SqlParameter("@Name", student.Name),
                registrationNo,
                new SqlParameter("@Status", student.Status),
                new SqlParameter("@Department", DBNull.Value),
                new SqlParameter("@AppliedOn", student.AppliedOn),
                joinedOn,
                leftOn,
                new SqlParameter("@DoB", Convert.ToDateTime(student.DateOfBirth)),
                new SqlParameter("@SiteUser", siteUser),
                lastStatus
            };
            return para;
        }
        public static SqlParameter[] para4OASubject(int id, SubjectsOfOA soa)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@Level", soa.Level),
                new SqlParameter("@Subject", soa.Name),
                new SqlParameter("@Grade", soa.Grade),
            };
            return para;
        }
        public static SqlParameter[] Para4FirstApplication(int id, NewApplication app)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@AppSubject", app.AppSubject),
                new SqlParameter("@AppBody", app.AppBody),
                new SqlParameter("@StatusId", app.StatusId),
                new SqlParameter("@Accepted", true)
            };
            return para;
        }
        public static SqlParameter[] Para4NewApplicants(int id, NewApplication app)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@AppSubject", app.AppSubject),
                new SqlParameter("@AppBody", app.AppBody),
                new SqlParameter("@StatusId", app.StatusId),
                new SqlParameter("@Accepted", DBNull.Value)
            };
            return para;
        }
        public static SqlParameter[] Para4JoiningLetter(int id, JoiningLetter let)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@PartnerId", let.PartnerId),
                new SqlParameter("@AppSubject", let.AppSubject),
                new SqlParameter("@AppBody", let.AppBody),
                new SqlParameter("@Date", let.Date),
                new SqlParameter("@Reviewed", true)
            };
            return para;
        }
        public static SqlParameter[] Para4NewJoiningLetter(int id, JoiningLetter let)
        {
            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@PartnerId", let.PartnerId),
                new SqlParameter("@AppSubject", let.AppSubject),
                new SqlParameter("@AppBody", let.AppBody),
                new SqlParameter("@Date", let.Date),
                new SqlParameter("@Reviewed", false)
            };
            return para;
        }
        public static SqlParameter[] Para4RegularApps(int id, RegularApplication reg)
        {
            SqlParameter managerId = reg.ManagerId == 0 ?
                new SqlParameter("@ManagerId", DBNull.Value) : new SqlParameter("@ManagerId", reg.ManagerId);

            SqlParameter managerNote = reg.ManagerNote == null ?
                new SqlParameter("@ManagerNote", DBNull.Value) : new SqlParameter("@ManagerNote", reg.ManagerNote);

            SqlParameter partnerNote = reg.PartnerNote == null ?
                new SqlParameter("@PartnerNote", DBNull.Value) : new SqlParameter("@PartnerNote", reg.PartnerNote);

            SqlParameter accepted = reg.Accepted == null ?
                new SqlParameter("@Accepted", DBNull.Value) : new SqlParameter("@Accepted", reg.Accepted);

            SqlParameter[] para = {
                new SqlParameter("@StudentId", id),
                managerId,
                new SqlParameter("@PartnerId", reg.PartnerId),
                new SqlParameter("@ApplicationTypeId", reg.AppTypeId),
                new SqlParameter("@ApplicationBody", reg.AppBody),
                new SqlParameter("@Date", reg.Date),
                managerNote,
                partnerNote,
                new SqlParameter("@StudentReview", reg.StudentReview),
                new SqlParameter("@ManagerReview", reg.ManagerReview),
                new SqlParameter("@PartnerReview", reg.PartnerReview),
                accepted
            };
            return para;
        }
        public static SqlParameter[] Para4HRSupervisory(int id, HRSupervisory sup)
        {
            SqlParameter managerId = sup.ManagerId == 0 ?
                new SqlParameter("@ManagerId", DBNull.Value) : new SqlParameter("@ManagerId", sup.ManagerId);
            SqlParameter to = sup.To.Year == 1905 ?
                new SqlParameter("@To", DBNull.Value) : new SqlParameter("@To", sup.To);

            SqlParameter[] para = {
                new SqlParameter("@Id", id),
                new SqlParameter("@PartnerId", sup.PartnerId),
                managerId,
                new SqlParameter("@DepartmentId", sup.DepartmentId),
                new SqlParameter("@StatusId", sup.StatusId),
                new SqlParameter("@From", sup.From),
                to
            };
            return para;
        }
        public static SqlParameter[] Para4UserTable(int id, string userType)
        {
            SqlParameter UserName, Role; UserName = Role = null;
            if (userType == "Partner")
            {
                UserName = new SqlParameter("@Name", "ep" + id);
                Role = new SqlParameter("@Role", "Partner");
            }
            else if (userType == "Manager")
            {
                UserName = new SqlParameter("@Name", "man" + id);
                Role = new SqlParameter("@Role", "Manager");
            }
            else if (userType == "Joining")
            {
                UserName = new SqlParameter("@Name", "emp" + id);
                Role = new SqlParameter("@Role", "Joining");
            }
            else
            {
                UserName = new SqlParameter("@Name", "emp" + id);
                Role = new SqlParameter("@Role", "Student");
            }

            SqlParameter[] para =
            {
                new SqlParameter("@StudentId", id),
                UserName,
                new SqlParameter("@Password", 1234),
                Role,
                new SqlParameter("@Retry", Convert.ToInt32(0)),
                new SqlParameter("@Status", DBNull.Value)
            };
            return para;
        }
        public static SqlParameter[] Para4Registration(StudentCV scv)
        {
            SqlParameter period = scv.Registration.PeriodOfCourse == 0 ?
                new SqlParameter("@Period", DBNull.Value) : new SqlParameter("@Period", scv.Registration.PeriodOfCourse);

            SqlParameter[] para = {
                new SqlParameter("@Id", scv.Id),
                new SqlParameter("@Date", scv.Registration.Date),
                period
            };
            return para;
        }
        public static SqlCommand ExecuteTransaction(string Query, SqlParameter[] param,
                                   SqlConnection con, SqlTransaction Tran)
        {
            string Q = Query;
            SqlCommand cmd = new SqlCommand(Query, con, Tran);
            if (param != null)
            {
                foreach (SqlParameter p in param)
                {
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }
    }
}

public class Registration
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int PeriodOfCourse { get; set; }
}

public class PartManInfo
{
    public int PartnerId { get; set; }
    public int ManagerId { get; set; }
}

public class Users
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public short Retry { get; set; }
    public string Status { get; set; }
}

public class StudentCV
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RegistrationNo { get; set; }
    public string Status { get; set; }
    public DateTime AppliedOn { get; set; }
    public DateTime JoinedOn { get; set; }
    public DateTime LeftOn { get; set; }
    public string FatherName { get; set; }
    public string MotherName { get; set; }
    public string Religion { get; set; }
    public string Gender { get; set; }
    public string MaritalStatus { get; set; }
    public string Nationality { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Blood { get; set; }

    public string ContactNo { get; set; }
    public string Email { get; set; }
    public string PresentAddress { get; set; }
    public string PermanentAddress { get; set; }
    public string EmergencyContactNo { get; set; }
    public string RelationWithEmergencyContact { get; set; }

    public string Objective { get; set; }
    public string ApplicationFor { get; set; }
    public string AppSubject { get; set; }
    public string AppBody { get; set; }

    public List<UniversityInfo> Education { get; set; }
    public List<SubjectsOfOA> SubjectsOA { get; set; }

    public List<ReferenceInfo> Reference { get; set; }
    public List<Experience> Experiences { get; set; }
    public CurrentPosition CurrentJob { get; set; }
    public List<PartnerMembership> PartnerMembership { get; set; }
    public List<PartnerPastPosition> PartnerPastPosition { get; set; }
    public List<PartnerCurrentPosition> PartnerCurrentPosition { get; set; }
    public List<HRSupervisory> SupervisingHistory { get; set; }
    public StudentApplications Applications { get; set; }
    public Registration Registration { get; set; }

}

public class PartnerMembership
{
    public string Id { get; set; }
    public string Organisation { get; set; }
}

public class PartnerPastPosition
{
    public string Position { get; set; }
    public string Organisation { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class PartnerCurrentPosition
{
    public string Position { get; set; }
    public string Organisation { get; set; }
    public DateTime From { get; set; }
}

public class SubjectsOfOA
{
    public string Level { get; set; }
    public string Name { get; set; }
    public string Grade { get; set; }
}

public class ReferenceInfo
{
    public string Name { get; set; }
    public string Designation { get; set; }
    public string Organisation { get; set; }
    public string ContactNo { get; set; }
    public string Relation { get; set; }
}

public class UniversityInfo
{
    public string Type { get; set; }
    public string Title { get; set; }
    public string Major { get; set; }
    public string Institute { get; set; }
    public string YearOfPass { get; set; }
    public string CGPA { get; set; }
    public string University { get; set; }
}

public class CurrentPosition
{
    public string Position { get; set; }
    public string Organisation { get; set; }
    public DateTime From { get; set; }
    public string Responsibilities { get; set; }
}

public class Experience
{
    public string Position { get; set; }
    public string Organisation { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Responsibilities { get; set; }
}

public class StudentApplications
{
    public NewApplication FirstApplication { get; set; }
    public JoiningLetter Joining { get; set; }
    public List<RegularApplication> Applications { get; set; }
}

public class RegularApplication
{
    public int Sl { get; set; }
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public int ManagerId { get; set; }
    public int AppTypeId { get; set; }
    public string AppBody { get; set; }
    public DateTime Date { get; set; }
    public string ManagerNote { get; set; }
    public string PartnerNote { get; set; }
    public bool StudentReview { get; set; }
    public bool ManagerReview { get; set; }
    public bool PartnerReview { get; set; }
    public bool? Accepted { get; set; }
}

public class HRSupervisory
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public int ManagerId { get; set; }
    public int DepartmentId { get; set; }
    public int StatusId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class JoiningLetter
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public string AppSubject { get; set; }
    public string AppBody { get; set; }
    public DateTime Date { get; set; }
}

public class NewApplication
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AppSubject { get; set; }
    public string AppBody { get; set; }
    public int StatusId { get; set; }
    public DateTime Date { get; set; }
}