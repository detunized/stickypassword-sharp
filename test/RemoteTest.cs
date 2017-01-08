// Copyright (C) 2017 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

using System;
using Moq;
using NUnit.Framework;
using RestSharp;

namespace StickyPassword.Test
{
    [TestFixture]
    class RemoteTest
    {
        public const string Username = "lebowski";
        public const string Password = "logjammin";
        public const string DeviceId = "ringer";

        [Test]
        public void GetEncryptedToken_sets_api_base_url()
        {
            var client = SetupClient("");

            Remote.GetEncryptedToken(Username, DeviceId, client.Object);

            client.VerifySet(x => x.BaseUrl = It.Is<Uri>(
                u => u.AbsoluteUri.Contains("stickypassword.com/SPCClient")));
        }

        [Test]
        public void GetEncryptedToken_sets_user_agent_with_device_id()
        {
            var client = SetupClient("");

            Remote.GetEncryptedToken(Username, DeviceId, client.Object);

            client.VerifySet(x => x.UserAgent = It.Is<string>(s => s.Contains(DeviceId)));
        }

        [Test]
        public void GetEncryptedToken_makes_post_request_to_specific_end_point()
        {
            var client = SetupClient("");

            Remote.GetEncryptedToken(Username, DeviceId, client.Object);

            client.Verify(x => x.Execute(It.Is<IRestRequest>(
                r => r.Method == Method.POST && r.Resource == "GetCrpToken")));
        }

        [Test]
        public void GetEncryptedToken_returns_response()
        {
            var resposne = "<xml></xml>";
            var client = SetupClient(resposne);

            Assert.That(Remote.GetEncryptedToken(Username, DeviceId, client.Object), Is.EqualTo(resposne));
        }

        //
        // Helpers
        //

        private static Mock<IRestClient> SetupClient(string response)
        {
            var mock = new Mock<IRestClient>();
            mock
                .Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns(SetupResponse(response).Object);
            return mock;
        }

        private static Mock<IRestResponse> SetupResponse(string response)
        {
            var mock = new Mock<IRestResponse>();
            mock.Setup(x => x.Content).Returns(response);
            return mock;
        }
    }
}
