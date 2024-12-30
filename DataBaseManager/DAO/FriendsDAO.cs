using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;

namespace DataBaseManager.DAO {

    public static class FriendsDAO {

        public static int StrikeUpFriendshipDAO(int idProfile1, int idProfile2) {
            LoggerManager logger = new LoggerManager(typeof(FriendsDAO));
            int operationResult = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Amistad newFriendship = new Amistad() {
                        idJugadorUno = idProfile1,
                        idJugadorDos = idProfile2
                    };

                    db.Amistad.Add(newFriendship);
                    db.SaveChanges();
                    operationResult = Constants.SUCCESSFUL_OPERATION;
                }
            } catch (EntityException entityException) {
                logger.LogError($"Error trying to register the friendship for profiles {idProfile1} and {idProfile2}. EntityException: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"Database error occurred while registering the friendship for profiles {idProfile1} and {idProfile2}. SqlException: {sqlException.Message}", sqlException);
            } catch (Exception generalException) {
                logger.LogError($"Unexpected error while registering the friendship for profiles {idProfile1} and {idProfile2}. Exception: {generalException.Message}", generalException);
            }
            return operationResult;
        }

        public static int DeleteFriendshipDAO(int idProfile1, int idProfile2) {
            LoggerManager logger = new LoggerManager(typeof(FriendsDAO));
            int operationResult = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Amistad friendshipToDelete = db.Amistad.FirstOrDefault(a =>
                        a.idJugadorUno == idProfile1 && a.idJugadorDos == idProfile2);

                    if (friendshipToDelete != null) {
                        db.Amistad.Remove(friendshipToDelete);
                        db.SaveChanges();
                        operationResult = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationResult = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while deleting the friendship between profiles {idProfile1} and {idProfile2}. Exception: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Database error occurred while deleting the friendship between profiles {idProfile1} and {idProfile2}. Exception: {sqlException.Message}", sqlException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while deleting the friendship between profiles {idProfile1} and {idProfile2}. Exception: {generalException.Message}", generalException);
            }
            return operationResult;
        }

        public static List<Perfil> GetFriendsDAO(int idProfile) {
            LoggerManager logger = new LoggerManager(typeof(FriendsDAO));
            List<Perfil> friendList = new List<Perfil>();
            Perfil operationFailed = new Perfil();
            operationFailed.idPerfil = Constants.FAILED_OPERATION;

            try {
                using (tripasEntities db = new tripasEntities()) {
                    List<Perfil> friends = (from friendship in db.Amistad
                                   where friendship.idJugadorUno == idProfile
                                   join profile in db.Perfil on friendship.idJugadorDos equals profile.idPerfil
                                   select profile).ToList();
                    friendList = friends;
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while retrieving the friend list for profile {idProfile}. Exception: {entityException.Message}", entityException);
                friendList.Add(operationFailed);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Database error occurred while retrieving the friend list for profile {idProfile}. Exception: {sqlException.Message}", sqlException);
                friendList.Add(operationFailed);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while retrieving the friend list for profile {idProfile}. Exception: {generalException.Message}", generalException);
                friendList.Add(operationFailed);
            }
            return friendList;
        }

        public static int IsFriendAlreadyAddedDAO(int idProfile1, int idProfile2) {
            LoggerManager logger = new LoggerManager(typeof(FriendsDAO));
            int operationResult = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingFriendship = db.Amistad.FirstOrDefault(a =>
                        a.idJugadorUno == idProfile1 && a.idJugadorDos == idProfile2);
                    if (existingFriendship != null) {
                        operationResult = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationResult = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while verifying friendship between profiles {idProfile1} and {idProfile2}. Exception: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Database error occurred while verifying friendship between profiles {idProfile1} and {idProfile2}. Exception: {sqlException.Message}", sqlException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while verifying friendship between profiles {idProfile1} and {idProfile2}. Exception: {generalException.Message}", generalException);
            }
            return operationResult;
        }

    }
}