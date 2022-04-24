using Selenium.utils.Utils;
using Selenium.utils.Enum;
using System;
using System.Linq;
using Test.Domain;
using Test.Mock;
using Xunit;
using System.Text.Json;
using FluentAssertions;

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

            //Download Documents
            User manager = UserFake.AllUsers().Where(x => x.IsManager && x.Id != user.Id).FirstOrDefault();
            var body = JsonSerializer.Serialize(aPIUtils.GetAccessDocumentBody(manager.Id, DocumentFake.Document1Id));

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200", "ok");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document1Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responseDownload.Message.Should().Be("ok");

            responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document2Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responseDownload.Message.Should().Be("ok");

            responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document3Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responseDownload.Message.Should().Be("ok");

            responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=" + DocumentFake.Document4Id, "");
            responseDownload.StatusCode.Should().Be(((int)StatusCode.Status200).ToString());
            responseDownload.Message.Should().Be("ok");
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
