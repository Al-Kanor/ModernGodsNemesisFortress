using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace NemesisFortress {
    public class Fortress {
        public int life = 1000;
    }
    
    public class Player : BasePlayer {
        // Position
        public float px = 0;
        public float py = 0;
        public float pz = 0;

        // Rotation
        public float rx = 0;
        public float ry = 0;
        public float rz = 0;
    }

    public class SpawnManager {
        public int currentEnemyId = 1;
    }

    [RoomType ("NemesisFortress")]
    public class GameCode : Game<Player> {
        Fortress fortress;
        SpawnManager spawnManager;

        // This method is called when an instance of your the game is created
        public override void GameStarted () {
            Console.WriteLine ("Game is started : " + RoomId);
            fortress = new Fortress ();
            spawnManager = new SpawnManager ();
            SpawnEnemy ();
        }

        public void SpawnEnemy () {
            System.Random random = new System.Random ();
            float spawnRate = 1;
            int spawnProba = 50;
            
            int rand = random.Next (0, 99);

            if (rand < spawnProba) {
                
                float x = 0, z = 0;
                /*
                 * Coordonnées de spawn (Carte vue du ciel, abscisses = x, ordonnées = z) :
                 * -100, [-100, 100] => Colonne de gauche
                 * 100, [-100, 100] => Colonne de droite
                 * [-100, 100], -100 => Ligne du bas
                 * [-100, 100], 100 => Ligne du haut
                 */
                if (0 == random.Next (0, 2)) {
                    // 1 chance sur 2 de spawn sur une colonne
                    x = 0 == random.Next (0, 2) ? -100 : 100;  // Colonne gauche / droite
                    z = random.Next (-100, 101);
                }
                else {
                    // 1 chance sur 2 de spawn sur une ligne
                    x = random.Next (-100, 101);
                    z = 0 == random.Next (0, 2) ? -100 : 100;  // Ligne haut / bas
                }

                string enemyType = "";
                rand = random.Next (0, 100);

                if (rand < 80) {   // 80% de chance de spawn un ennemi simple
                    enemyType = "simple";
                }
                else if (rand < 88) {  // 8% de chance de spawn un ennemi géant
                    enemyType = "giant";
                }
                else if (rand < 94) {  // 6% de chance de spawn un samouraï
                    enemyType = "samurai";
                }
                else if (rand < 98) {  // 4% de chance de spawn une araignée
                    enemyType = "spider";
                }
                else {  // 2% de chance de spawn un gorille
                    enemyType = "gorilla";
                }

                Broadcast ("Enemy Spawn", enemyType, spawnManager.currentEnemyId++, x.ToString(), z.ToString());
            }

            ScheduleCallback (SpawnEnemy, (int) (spawnRate * 1000));
        }

        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed () {
            Console.WriteLine ("RoomId : " + RoomId);
        }

        // This method is called whenever a player joins the game
        public override void UserJoined (Player _player) {
            foreach (Player player in Players) {
                if (player.ConnectUserId != _player.ConnectUserId) {
                    player.Send ("Player Joined", _player.ConnectUserId, 0, 0);
                    _player.Send ("Player Joined", player.ConnectUserId, player.px, player.py, player.pz);
                }
            }
        }

        // This method is called when a player leaves the game
        public override void UserLeft (Player player) {
            Broadcast ("PlayerLeft", player.ConnectUserId);
        }

        // This method is called when a player sends a message into the server code
        public override void GotMessage (Player _player, Message message) {
            Console.WriteLine ("Message from client : " + _player.ConnectUserId + " : " + message.Type);
            switch (message.Type) {
                case "Chat":
                    foreach (Player player in Players) {
                        if (player.ConnectUserId != _player.ConnectUserId) {
                            player.Send (message.Type, _player.ConnectUserId, message.GetString (0));
                        }
                    }
                    break;
                case "Enemy Damage":
                    Broadcast (message.Type, message.GetInt (0), message.GetInt(1));
                    break;
                case "Fortress Damage":
                    fortress.life -= message.GetInt (0);
                    if (fortress.life < 0) fortress.life = 0;
                    Broadcast (message.Type, fortress.life);
                    break;
                case "Player Position":
                    /*
                    _player.px = message.GetFloat (0);
                    _player.py = message.GetFloat (1);
                    _player.pz = message.GetFloat (2);
                    _player.rx = message.GetFloat (3);
                    _player.ry = message.GetFloat (4);
                    _player.rz = message.GetFloat (5);
                    Broadcast (message.Type, _player.ConnectUserId, _player.px, _player.py, _player.pz, _player.rx, _player.ry, _player.rz);
                    */
                    Broadcast (message.Type, _player.ConnectUserId, message.GetFloat (0), message.GetFloat (1), message.GetFloat (2), message.GetFloat (3), message.GetFloat (4), message.GetFloat (5));
                    break;
            }
        }
    }
}