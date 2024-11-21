using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TripasService.Utils;
using static TripasService.Utils.GameEnums;

namespace TripasService.Logic {
    [DataContract]
    public class Node {
        // Identificador único del nodo
        [DataMember]
        public string Id { get; set; }

        // Coordenadas del nodo (útil para renderizado gráfico)
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        // Estado del nodo (ocupado, libre, etc.)
        [DataMember]
        public GameEnums.NodeStatus Status { get; set; }

        // Jugador que controla este nodo (si aplica)
        [DataMember]
        public Profile Owner { get; set; }

        // Constructor completo
        public Node(string id, double x, double y, GameEnums.NodeStatus status = GameEnums.NodeStatus.Free) {
            Id = id;
            X = x;
            Y = y;
            Status = status;
        }

        // Constructor por defecto (requerido para serialización)
        public Node() {
            Id = Guid.NewGuid().ToString();
            Status = GameEnums.NodeStatus.Free;
        }

        // Método para cambiar el estado del nodo
        public void ChangeOwnership(Profile newOwner) {
            Owner = newOwner;
            Status = newOwner != null ? NodeStatus.Occupied : NodeStatus.Free;
        }

        // Método para verificar si dos nodos están conectados
        public bool IsConnectedTo(Node otherNode) {
            // La lógica de conexión dependerá de tu juego específico
            // Podrías implementar una distancia máxima, por ejemplo
            double distance = Math.Sqrt(
                Math.Pow(X - otherNode.X, 2) +
                Math.Pow(Y - otherNode.Y, 2)
            );

            // Por ejemplo, considerar conectados si la distancia es menor a 100 unidades
            return distance <= 100;
        }

        // Método para obtener la distancia entre dos nodos
        public double DistanceTo(Node otherNode) {
            return Math.Sqrt(
                Math.Pow(X - otherNode.X, 2) +
                Math.Pow(Y - otherNode.Y, 2)
            );
        }

        // Sobrescribir Equals para comparación de nodos
        public override bool Equals(object obj) {
            if (obj is Node otherNode) {
                return Id == otherNode.Id;
            }
            return false;
        }

        // Sobrescribir GetHashCode para uso en colecciones
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        // ToString para depuración
        public override string ToString() {
            return $"Node {Id} at ({X}, {Y}) - Status: {Status}";
        }
    }
}

