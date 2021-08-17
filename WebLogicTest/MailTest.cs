using AkaDB.MySql;
using Common.Entities.Mail;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebLogicTest
{
    class MailTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task GetMailInfoTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateMail(AkaEnum.MailType.Common, accountDb, userDb, 7);
                    var mailInfo = await mail.GetMailInfo("KR");
                }
            }
        }

        [Test]
        public async Task PrivateMailReadTest1()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateMail(AkaEnum.MailType.Private, accountDb, userDb, 7);
                    await mail.MailRead(1, true);
                }
            }
        }

        [Test]
        public async Task PrivateMailReadTest2()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateMail(AkaEnum.MailType.Private, accountDb, userDb, 7);
                    var result = await mail.MailRead(7, true);
                }
            }
        }

        [Test]
        public async Task PublicMailReadTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateMail(AkaEnum.MailType.Public, accountDb, userDb, 7);
                    var result = await mail.MailRead(77283, true);
                }
            }
        }

        [Test]
        public async Task SystemMailReadTest1()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateSystemMail(accountDb, userDb, 7);
                    var result = await mail.MailRead(242, true);
                }
            }
        }

        [Test]
        public async Task PrivateMailReadAllTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateMail(AkaEnum.MailType.Private, accountDb, userDb, 7);
                    var mailIds = new List<uint>();
                    mailIds.Add(1);
                    mailIds.Add(2);
                    mailIds.Add(3);
                    mailIds.Add(4);
                    mailIds.Add(5);
                    var result = await mail.MailReadAll(mailIds);
                }
            }
        }

        [Test]
        public async Task PublicMailReadAllTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateMail(AkaEnum.MailType.Public, accountDb, userDb, 7);
                    var mailIds = new List<uint>();
                    mailIds.Add(7281);
                    mailIds.Add(7282);
                    var result = await mail.MailReadAll(mailIds);
                }
            }
        }

        [Test]
        public async Task SystemMailReadAllTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mailIds = new List<uint>();
                    mailIds.Add(242);
                    mailIds.Add(243);
                    var mail = MailFactory.CreateSystemMail(accountDb, userDb, 7);
                    var result = await mail.MailReadAll(mailIds);
                }
            }
        }

        [Test]
        public async Task SystemMailDeleteAllTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreateMail(AkaEnum.MailType.System, accountDb, userDb, 7);
                    var mailIds = new List<uint>();
                    mailIds.Add(242);
                    mailIds.Add(243);
                    var result = await mail.MailDeleteAll(mailIds);
                }
            }
        }

        [Test]
        public async Task PublicMailUpdateTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var mail = MailFactory.CreatePublicMail(accountDb, userDb, 7);
                    var result = await mail.MailUpdate("KR");
                }
            }
        }

        [Test]
        public async Task PrivateMailUpdateTest()
        {
            using (var userDb = new DBContext(7))
            {
                var mail = MailFactory.CreatePrivateMail(null, userDb, 7);
                var result = await mail.MailUpdate();
            }
        }

        [Test]
        public async Task PrivateMailDeleteAllTest()
        {
            using (var userDb = new DBContext(7))
            {
                var mail = MailFactory.CreatePrivateMail(null, userDb, 7);
                var mailIds = new List<uint>();
                mailIds.Add(1);
                mailIds.Add(2);
                mailIds.Add(3);
                mailIds.Add(4);
                mailIds.Add(5);
                mailIds.Add(6);
                mailIds.Add(7);
                var result = await mail.MailDeleteAll(mailIds);
            }
        }

        [Test]
        public async Task PublicMailDeleteAllTest()
        {
            using (var userDb = new DBContext(7))
            {
                var mail = MailFactory.CreatePublicMail(null, userDb, 7);
                var mailIds = new List<uint>();
                mailIds.Add(77280);
                mailIds.Add(77281);
                mailIds.Add(77282);
                mailIds.Add(77283);
                var result = await mail.MailDeleteAll(mailIds);
            }
        }
    }
}
