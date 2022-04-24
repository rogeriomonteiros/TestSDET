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
            var body =  JsonSerializer.Serialize(APIUtils.GetLoginBody(user.Email, user.Password));
            
            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200", "ok");

            var responselogin = aPIUtils.APIJson("https://api.example.com/login", body );
            responselogin.StatusCode.Should().Be( ((int)StatusCode.Status200).ToString());
            responselogin.Message.Should().Be("ok");

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "user not authorized");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=1", "");
            responseDownload.StatusCode.Should().Be( ((int)StatusCode.Status401).ToString() );
            responseDownload.Message.Should().Be("user not authorized");

        }
        
        [Fact]
        public void Manager_can_grant_an_employee_access_to_document()
        {
            User user = UserFake.AllUsers().Where(x => x.Documents != null && !x.Documents.Select(b => b.Id).Contains(DocumentFake.Document1Id)).FirstOrDefault();
            var body =  JsonSerializer.Serialize(APIUtils.GetLoginBody(user.Email, user.Password));
            
            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("200", "ok");

            var responselogin = aPIUtils.APIJson("https://api.example.com/login", body );
            responselogin.StatusCode.Should().Be( ((int)StatusCode.Status200).ToString());
            responselogin.Message.Should().Be("ok");

            //Preparing response because there is no API to test. In real test we must remove this command
            aPIUtils.SetResponse("401", "user not authorized");

            var responseDownload = aPIUtils.APIJson("https://api.example.com/docs/download?id=1", "");
            responseDownload.StatusCode.Should().Be( ((int)StatusCode.Status401).ToString() );
            responseDownload.Message.Should().Be("user not authorized");

        }
    }
}
