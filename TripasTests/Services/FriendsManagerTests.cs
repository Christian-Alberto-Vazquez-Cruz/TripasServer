/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TripasTests.Services {
    public class FriendsManagerTest {

        // Prueba original
        [Fact]
        public void FriendsTest() {
            ProxyTripas.FriendsManagerClient friendsManager = new ProxyTripas.FriendsManagerClient();
            int resultExpected = 1;
            int resultObtained = friendsManager.AddFriend(1003, 1004);
            Assert.Equal(resultExpected, resultObtained);
        }

        [Fact]
        public void FriendsExceptionTest() {
            ProxyTripas.FriendsManagerClient friendManager = new ProxyTripas.FriendsManagerClient();
            int resultExpected = -1;
            int resultObtained = friendManager.AddFriend(1, 2);
            Assert.Equal(resultExpected, resultObtained);
        }

        // Nuevas pruebas unitarias

        // Prueba para DeleteFriend
        [Fact]
        public void DeleteFriendTest() {
            ProxyTripas.FriendsManagerClient friendsManager = new ProxyTripas.FriendsManagerClient();
            int resultExpected = 1; // Se espera que la operación se complete con éxito
            int resultObtained = friendsManager.DeleteFriend(1003, 1004);
            Assert.Equal(resultExpected, resultObtained);
        }

        [Fact]
        public void DeleteFriendExceptionTest() {
            ProxyTripas.FriendsManagerClient friendsManager = new ProxyTripas.FriendsManagerClient();
            int resultExpected = -1; // Error, no se puede eliminar la amistad
            int resultObtained = friendsManager.DeleteFriend(1003, 1005); // Perfil no existe o no hay amistad
            Assert.Equal(resultExpected, resultObtained);
        }

        /* Prueba para GetFriends
        [Fact]
        public void GetFriendsTest() {
            ProxyTripas.FriendsManagerClient friendsManager = new ProxyTripas.FriendsManagerClient();
            var expectedFriends = new List<Profile> {
                new Profile { IdProfile = 1004, Username = "FriendUser1" },
                new Profile { IdProfile = 1005, Username = "FriendUser2" }
            };

            var resultObtained = friendsManager.GetFriends(1003);

            Assert.NotNull(resultObtained);
            Assert.Equal(expectedFriends.Count, resultObtained.Count);
            Assert.Equal(expectedFriends[0].IdProfile, resultObtained[0].IdProfile);
            Assert.Equal(expectedFriends[0].Username, resultObtained[0].Username);
        }

        [Fact]
        public void GetFriendsEmptyTest() {
            ProxyTripas.FriendsManagerClient friendsManager = new ProxyTripas.FriendsManagerClient();
            var resultObtained = friendsManager.GetFriends(9999); // Un perfil que no tiene amigos

            Assert.Empty(resultObtained);
        }

        // Excepciones en AddFriend
        [Fact]
        public void AddFriendDatabaseExceptionTest() {
            ProxyTripas.FriendsManagerClient friendsManager = new ProxyTripas.FriendsManagerClient();

            // Simulamos una excepción en el DAO de la base de datos
            int resultExpected = -1; // Error por alguna falla en la base de datos
            int resultObtained = friendsManager.AddFriend(1003, 1105); // Aquí puedes probar con un ID inválido
            Assert.Equal(resultExpected, resultObtained);
        }

        [Fact]
        public void AddFriendNoMatchTest() {
            ProxyTripas.FriendsManagerClient friendsManager = new ProxyTripas.FriendsManagerClient();

            // Intentamos agregar una amistad con perfiles que no existen
            int resultExpected = -1; // No se puede agregar
            int resultObtained = friendsManager.AddFriend(9999, 10000); // IDs que no existen
            Assert.Equal(resultExpected, resultObtained);
        }
    }
}*/
