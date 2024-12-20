﻿using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;

namespace DataBaseManager.DAO {
    public static class LeaderboardDAO {
        public static List<Perfil> GetHighestScoresDAO() {
            LoggerManager logger = new LoggerManager(typeof(LeaderboardDAO));
            Perfil operationFailed = new Perfil() {
                idPerfil = Constants.FAILED_OPERATION
            };
            List<Perfil> bestPlayersList = new List<Perfil>();
            try {
                using (tripasEntities db = new tripasEntities()) {
                    List<Perfil> profiles = db.Perfil.OrderByDescending(perfil => perfil.puntaje)
                                            .Take(Constants.HOW_MANY_SCORES)
                                            .ToList();
                    bestPlayersList = profiles;
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while retrieving the highest scoring players. Exception: {entityException.Message}", entityException);
                bestPlayersList.Add(operationFailed);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Database error occurred while retrieving the highest scoring players. Exception: {sqlException.Message}", sqlException);
                bestPlayersList.Add(operationFailed);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while retrieving the highest scoring players. Exception: {generalException.Message}", generalException);
                bestPlayersList.Add(operationFailed);
            }

            return bestPlayersList;
        }

    }
}