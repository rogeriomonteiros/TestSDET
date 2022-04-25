using FluentAssertions;
using Selenium.utils.Enum;
using Selenium.utils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Test.Domain;
using Test.Mock;
using Xunit;

namespace Test
{
    public class TestSDET
    {
        private APIUtils aPIUtils;
        public TestSDET()
        {
            aPIUtils = new APIUtils();
        }

        [Fact]
        public void Should_only_allow_authorized_users_to_download_documents()
        {
            User user = UserFake.AllUsers().Where(x => x.Documents != null && !x.Documents.Select(b => b.Id).Contains(DocumentFake.Document1Id)).FirstOrDefault();
            this.LoginAPI(user);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "user not authorized");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document1Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            responseDownload.Message.Should().Be("user not authorized");

        }

        [Fact]
        public void Manager_can_grant_an_employee_access_to_document()
        {
            User user = UserFake.AllUsers().Where(x => x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Employee without Document1 access
            User employee = UserFake.AllUsers().Where(x => x.Documents != null && !x.Documents.Select(b => b.Id).Contains(DocumentFake.Document1Id)).FirstOrDefault();
            var body = JsonSerializer.Serialize(aPIUtils.GetAccessDocumentBody(employee.Id, DocumentFake.Document1Id));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200", "ok");

            var response = aPIUtils.APIJson("https://api.example.com/accesses/add", body);
            response.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            response.Message.Should().Be("ok");

            this.LoginAPI(employee);

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document1Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responseDownload.Message.Should().Be("ok");

        }

        [Fact]
        public void Manager_can_not_grant_access_to_another_manger()
        {
            User user = UserFake.AllUsers().Where(x => x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Find another manager
            User manager = UserFake.AllUsers().Where(x => x.IsManager && x.Id != user.Id).FirstOrDefault();
            var body = JsonSerializer.Serialize(aPIUtils.GetAccessDocumentBody(manager.Id, DocumentFake.Document1Id));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("400", "manager can not guant access to another manager");

            var response = aPIUtils.APIJson("https://api.example.com/accesses/add", body);
            response.StatusCode.Should().Be(((int)StatusCode.Status400).ToString());
            response.Message.Should().Be("manager can not guant access to another manager");
        }
        
        [Fact]
        public void Manager_can_access_all_documents()
        {
            User user = UserFake.AllUsers().Where(x => x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200",  JsonSerializer.Serialize(DocumentFake.AllDocuments()));

            var response = aPIUtils.APIJson("https://api.example.com/docs", "");
            response.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            List<Document> documents = (List<Document>)JsonSerializer.Deserialize(response.Message, typeof(List<Document>));
            documents.Count().Should().Be(4);
        }

        [Fact]
        public void Manager_can_remove_an_employee_access_to_document()
        {
            User user = UserFake.AllUsers().Where(x => x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Employee without Document1 access
            User employee = UserFake.AllUsers().Where(x => x.Documents != null && !x.Documents.Select(b => b.Id).Contains(DocumentFake.Document1Id)).FirstOrDefault();
            var body = JsonSerializer.Serialize(aPIUtils.GetAccessDocumentBody(employee.Id, DocumentFake.Document1Id));


            var response = aPIUtils.APIJson("https://api.example.com/accesses/remove", body);
            
            this.LoginAPI(employee);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document1Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            responseDownload.Message.Should().Be("not authorized");
        }
        
        [Fact]
        public void Manager_can_not_remove_an_manager_access_to_document()
        {
            User user = UserFake.AllUsers().Where(x => x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Find another manager
            User manager = UserFake.AllUsers().Where(x => x.IsManager && x.Id != user.Id).FirstOrDefault();
            var body = JsonSerializer.Serialize(aPIUtils.GetAccessDocumentBody(manager.Id, DocumentFake.Document1Id));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            var response = aPIUtils.APIJson("https://api.example.com/accesses/remove", body);
            response.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            response.Message.Should().Be("not authorized");
        }

        [Fact]
        public void Employee_can_only_view_documents_he_she_has_access_to()
        {
            User user = UserFake.AllUsers().Where(x => x.Documents != null && !x.Documents.Select(b => b.Id).Contains(DocumentFake.Document1Id)).FirstOrDefault();
            this.LoginAPI(user);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200", "ok");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document1Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responseDownload.Message.Should().Be("ok");

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document4Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            responseDownload.Message.Should().Be("not authorized");
        }
        
        [Fact]
        public void Employee_can_not_remove_access_to_another_employee()
        {
            User user = UserFake.AllUsers().Where(x => !x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Find another Employee
            User employee = UserFake.AllUsers().Where(x => !x.IsManager && x.Id != user.Id).FirstOrDefault();
            var body = JsonSerializer.Serialize(aPIUtils.GetAccessDocumentBody(employee.Id, DocumentFake.Document1Id));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            var response = aPIUtils.APIJson("https://api.example.com/accesses/remove", body);
            response.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            response.Message.Should().Be("not authorized");
        }
        
        [Fact]
        public void Employee_can_not_grant_access_to_another_employee()
        {
            User user = UserFake.AllUsers().Where(x => !x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Find another Employee
            User employee = UserFake.AllUsers().Where(x => !x.IsManager && x.Id != user.Id).FirstOrDefault();
            var body = JsonSerializer.Serialize(aPIUtils.GetAccessDocumentBody(employee.Id, DocumentFake.Document1Id));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            var response = aPIUtils.APIJson("https://api.example.com/accesses/add", body);
            response.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            response.Message.Should().Be("not authorized");
        }
        
        [Theory]
        [MemberData(nameof(Documents))]
        public void Manager_can_download_every_document(Document document, User manager)
        {
            this.LoginAPI(manager);
            
            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200", "ok");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + document.Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responseDownload.Message.Should().Be("ok");
        }
        
        [Fact]
        public void Should_not_allow_different_arguments_on_query_params()
        {
            User user = UserFake.AllUsers().Where(x => !x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("400", "bad request");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?1=1", "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status400).ToString());
            responseDownload.Message.Should().Be("bad request");
        }

        [Theory]
        [InlineData("aaaaa")]
        [InlineData("123456")]
        [InlineData("manager@leantech.com")]
        public void Try_to_access_with_weak_password(string password)
        {
            User user = UserFake.Manager1();

            var body = JsonSerializer.Serialize(aPIUtils.GetLoginBody(user.Email, password));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            var responselogin = aPIUtils.APIJson("https://api.example.com/login", body);
            responselogin.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            responselogin.Message.Should().Be("not authorized");

        }

        [Theory]
        [MemberData(nameof(LoginTries))]
        public void Should_limit_the_number_of_login_tries(User user, string password, string statusCode, string message)
        {
            var body = JsonSerializer.Serialize(aPIUtils.GetLoginBody(user.Email, password));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse(statusCode, message);

            var responselogin = aPIUtils.APIJson("https://api.example.com/login", body);
            responselogin.StatusCode.Should().Be(statusCode);
            responselogin.Message.Should().Be(message);
        }

        [Fact]
        public void Should_not_list_access()
        {
            User user = UserFake.AllUsers().Where(x => !x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("404", "not found");

            var response = aPIUtils.APIJson("https://api.example.com/accesses/", "");
            response.StatusCode.Should().Be(((int)StatusCode.Status404).ToString());
            response.Message.Should().Be("not found");
        }
        
        [Fact]
        public async Task Login_should_expire_after_30_seconds()
        {
            User user = UserFake.AllUsers().Where(x => !x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            await Task.Delay(15000);

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + user.Documents.First().Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            responseDownload.Message.Should().Be("not authorized");
        }
        
        [Fact]
        public void Employee_Should_not_download_file_that_he_she_has_not_asscess_to()
        {
            User user = UserFake.AllUsers().Where(x => !x.IsManager).FirstOrDefault();
            this.LoginAPI(user);

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "not authorized");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document3Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status401).ToString());
            responseDownload.Message.Should().Be("not authorized");
        }

        public static IEnumerable<object[]> LoginTries()
        {
            return new List<object[]>
            {
                new object[] { UserFake.Manager1(), "manager@leantech.com", "401", "not authorized" },
                new object[] { UserFake.Manager1(), "@12345", "401", "not authorized" },
                new object[] { UserFake.Manager1(), "Roger123", "400", "user temporarily blocked" },
            };
        }

        public static IEnumerable<object[]> Documents()
        {
            return new List<object[]>
            {
                new object[] { DocumentFake.Document1(), UserFake.Manager1() },
                new object[] { DocumentFake.Document2(), UserFake.Manager1() },
                new object[] { DocumentFake.Document3(), UserFake.Manager1() },
                new object[] { DocumentFake.Document4(), UserFake.Manager1() },
            };
        }

        private void LoginAPI(User user)
        {
            var body = JsonSerializer.Serialize(aPIUtils.GetLoginBody(user.Email, user.Password));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200", "ok");

            var responselogin = aPIUtils.APIJson("https://api.example.com/login", body);
            responselogin.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responselogin.Message.Should().Be("ok");

        }
    }
}
