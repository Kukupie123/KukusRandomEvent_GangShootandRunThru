using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
namespace KukuEvent
{
    public class Gang : Script
    {
        private List<Ped> ballas; //List of thugs in ballas team
        private List<Ped> groves; //List of thugs in groves team

        Dictionary<Ped, int> enterCarCounterB;
        Dictionary<Ped, int> enterCarCounterG;

        Dictionary<Ped, int> driveAwayCounterB;
        Dictionary<Ped, int> driveAwayCounterG;

        public bool eventActive = false; //used to make sure that we dont retrigger the event twice
        public bool missionActive = false;
        int startCHeck = 0;



        //Responsible for initializing the thugs
        public void instantiateGangs(int totalThugPerTeam, Vector3 spawnB, Vector3 spawnG)
        {
            eventActive = true;
            deleteThugs(); //to be safe we delete our old ones
            ballas = null;
            groves = null;

            //Counters are used to count the value, once it reaches a thrshold we execute another action
            driveAwayCounterB = new Dictionary<Ped, int>();
            driveAwayCounterG = new Dictionary<Ped, int>();
            enterCarCounterB = new Dictionary<Ped, int>();
            enterCarCounterG = new Dictionary<Ped, int>();

            //Sometime game skips initiation for some reason prolly frame skips and etc so we keep a loop to exit when everything has been initialized
            while (true)
            {
                if (ballas == null)
                {
                    ballas = new List<Ped>();
                }
                if (groves == null)
                {
                    groves = new List<Ped>();
                }
                if (enterCarCounterB == null)
                {
                    enterCarCounterB = new Dictionary<Ped, int>();
                }
                if (enterCarCounterG == null)
                {
                    enterCarCounterG = new Dictionary<Ped, int>();
                }
                if (driveAwayCounterB == null)
                {
                    driveAwayCounterB = new Dictionary<Ped, int>();
                }
                if (driveAwayCounterG == null)
                {
                    driveAwayCounterG = new Dictionary<Ped, int>();
                }
                if (ballas != null && groves != null && enterCarCounterG != null && enterCarCounterB != null && driveAwayCounterB != null && driveAwayCounterG != null)
                {
                    break;
                }
            }

            //random used to create different random models based on the output 
            Random random = new Random();
            //Spawning
            Model thug = null;
            for (int i = 0; i < totalThugPerTeam; i++)
            {
                int modelToUse = random.Next(1, 5);
                switch (modelToUse)
                {
                    case 1:
                        thug = new Model(PedHash.BallaEast01GMY);
                        break;
                    case 2:
                        thug = new Model(PedHash.BallaOrig01GMY);
                        break;
                    case 3:
                        thug = new Model(PedHash.Ballas01GFY);
                        break;
                    case 4:
                        thug = new Model(PedHash.Ballasog);
                        break;
                    case 5:
                        thug = new Model(PedHash.BallaSout01GMY);
                        break;
                }
                //Checking if the model is valid
                if (thug.IsValid)
                {
                    while (true) //If model is loaded then exit, else request and loop 
                    {
                        if (thug.IsLoaded)
                        {
                            break;
                        }
                        else
                        {
                            thug.Request(100);
                            Wait(0);
                        }
                    }

                    ballas.Add(World.CreatePed(thug, spawnB.Around(5)));
                    while (!ballas[i].Exists()) //THIS IS WHERE THE GAME CRASHES AFTER FEW RUNS PREVIOUSLY
                    {
                        Wait(0);
                    }
                }
            }

            //Doing the same for Groves
            for (int i = 0; i < totalThugPerTeam; i++)
            {
                int modelToUse = random.Next(1, 5);
                switch (modelToUse)
                {
                    case 1:
                        thug = new Model(PedHash.Famca01GMY);
                        break;
                    case 2:
                        thug = new Model(PedHash.Famdd01);
                        break;
                    case 3:
                        thug = new Model(PedHash.Famdnf01GMY);
                        break;
                    case 4:
                        thug = new Model(PedHash.Famfor01GMY);
                        break;
                    case 5:
                        thug = new Model(PedHash.Families01GFY);
                        break;
                }
                if (thug.IsValid)
                {
                    while (true)
                    {
                        if (thug.IsLoaded)
                        {
                            break;
                        }
                        else
                        {
                            thug.Request(100);
                            Wait(0);
                        }
                    }
                    groves.Add(World.CreatePed(thug, spawnG.Around(5)));

                    while (!groves[i].Exists())//THIS IS WHERE THE GAME CRASHES AFTER FEW RUNS
                    {
                        Wait(0);
                    }
                }

            }
            thug.MarkAsNoLongerNeeded();

        }

        private void assignTask(List<Ped> thugs, List<Ped> victim)
        {
            if (safetyCheck() == false)
            {
                return;
            }
            List<Ped> alive = thugsalive(thugs);
            List<Ped> victims = thugsalive(victim);
            Random rand = new Random();
            try
            {
                foreach (Ped p in alive)
                {
                    if (victims.Count < 1)
                    {
                        break;
                    }
                    int againtst = rand.Next(0, victim.Count);
                    p.Task.FightAgainst(victims[againtst]);
                    victims.RemoveAt(againtst);
                }

            }
            catch
            {
            }
        }

        /**
         * Call after we instantiate the ballas list. Tihs function will set the stats, etc to make them ready for fighrting
         */
        public void postInstantiation()
        {
            //Relationship indexes
            int groupBallas = Function.Call<int>(Hash.GET_HASH_KEY, "AMBIENT_GANG_BALLAS");
            int groupGrove = Function.Call<int>(Hash.GET_HASH_KEY, "AMBIENT_GANG_FAMILY");
            int cop = Function.Call<int>(Hash.GET_HASH_KEY, "COP");

            Random rand = new Random();

            //Giving random weapoons to each gang member ped
            foreach (Ped i in ballas)
            {
                int weapon = rand.Next(1, 5);
                switch (weapon)
                {
                    case 1:
                        i.Weapons.Give(WeaponHash.VintagePistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.CombatPDW, 100, true, true);
                        break;
                    case 2:
                        i.Weapons.Give(WeaponHash.APPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultShotgun, 900, true, true);
                        break;
                    case 3:
                        i.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.BullpupShotgun, 900, true, true);
                        break;
                    case 4:
                        i.Weapons.Give(WeaponHash.MicroSMG, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultrifleMk2, 800, true, true);
                        break;
                    case 5:
                        i.Weapons.Give(WeaponHash.Pistol50, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultSMG, 5, true, true);
                        break;
                }
                //Setting up relatinships, stats, blips etc
                i.Accuracy = rand.Next(40, 100);
                i.RelationshipGroup = groupBallas;

                Function.Call(Hash.SET_PED_COMBAT_ABILITY, i, rand.Next(30, 100)); // random attack power
                Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, i, 0, 0); //ped wont flee

                i.MaxHealth = 200000;
                i.Armor = 200000;
                i.Health = 200000;

                i.AddBlip();
                i.CurrentBlip.Color = BlipColor.Blue;
                i.CurrentBlip.Name = "Ballas";
                i.CurrentBlip.Scale = 0.6f;

                i.DrivingStyle=DrivingStyle.AvoidTrafficExtremely;
            }

            //Doing the same for grove
            foreach (Ped i in groves)
            {
                int weapon = rand.Next(1, 5);
                switch (weapon)
                {
                    case 1:
                        i.Weapons.Give(WeaponHash.Pistol50, 100, true, true);

                        i.Weapons.Give(WeaponHash.CombatMG, 100, true, true);
                        break;
                    case 2:
                        i.Weapons.Give(WeaponHash.APPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultShotgun, 900, true, true);
                        break;
                    case 3:
                        i.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.SpecialCarbine, 900, true, true);
                        break;
                    case 4:
                        i.Weapons.Give(WeaponHash.HeavyPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultrifleMk2, 800, true, true);
                        break;
                    case 5:
                        i.Weapons.Give(WeaponHash.DoubleActionRevolver, 100, true, true);

                        i.Weapons.Give(WeaponHash.PistolMk2, 5, true, true);
                        break;
                }
                i.Accuracy = rand.Next(40, 100);
                i.RelationshipGroup = groupGrove;



                Function.Call(Hash.SET_PED_COMBAT_ABILITY, i, rand.Next(30, 100)); // random attack power
                Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, i, 0, 0); //ped wont flee
                i.MaxHealth = 200000;
                i.Armor = 200000;
                i.Health = 200000;
                
                i.AddBlip();
                i.CurrentBlip.Color = BlipColor.Green;
                i.CurrentBlip.Name = "Grove";
                i.CurrentBlip.Scale = 0.6f;
                i.DrivingStyle=DrivingStyle.AvoidTrafficExtremely;
            }
            //Modifying the relationship
            World.SetRelationshipBetweenGroups(Relationship.Hate, groupGrove, groupBallas);
            World.SetRelationshipBetweenGroups(Relationship.Hate, groupBallas, groupGrove);
            if (Main.friendlyGrove == 1)
            {
                World.SetRelationshipBetweenGroups(Relationship.Companion, groupGrove, 0x6F0783F5);
            }
            else
            {
                World.SetRelationshipBetweenGroups(Relationship.Neutral, groupGrove, 0x6F0783F5);
            }

            World.SetRelationshipBetweenGroups(Relationship.Hate, cop, groupBallas);
            World.SetRelationshipBetweenGroups(Relationship.Hate, cop, groupGrove);

            World.SetRelationshipBetweenGroups(Relationship.Hate, groupGrove, cop);
            World.SetRelationshipBetweenGroups(Relationship.Hate, groupBallas, cop);


            //COUNTERS 
            foreach (Ped p in ballas)
            {
                enterCarCounterB.Add(p, 0);
                driveAwayCounterB.Add(p, 0);
            }
            foreach (Ped p in groves)
            {
                enterCarCounterG.Add(p, 0);
                driveAwayCounterG.Add(p, 0);
            }

            //COMPLETE NOW THEY SHALL FIGHT. DO NOT USE BLOCK TEMPORARY TASK AS THE FIGHT IS SUPPOSED TO BE NATURAL AND COME AS A TEMP TASK


        }

        /**
         * This method checks for dead thugs or out of range from player thugs and detaches them and setting the ped value to null
         */
        public void onTickClean(Ped player, int cleanupDistance)
        {
            //safety check
            if (ballas == null || groves == null || player == null)
            {
                return;
            }

            //Responsible to check if the dead thugs are deleted once they die
            for (int i = 0; i < ballas.Count; i++)
            {
                if (ballas[i] != null)
                {
                    if (ballas[i].IsDead || ballas[i].Position.DistanceTo2D(player.Position) > cleanupDistance)
                    {
                        ballas[i].CurrentBlip.Remove();
                        ballas[i].MarkAsNoLongerNeeded();
                        ballas[i].Detach();
                        ballas[i] = null;
                    }
                }
            }
            for (int i = 0; i < groves.Count; i++)
            {
                if (groves[i] != null)
                {
                    if (groves[i].IsDead || groves[i].Position.DistanceTo2D(player.Position) > cleanupDistance)
                    {
                        groves[i].CurrentBlip.Remove();
                        groves[i].MarkAsNoLongerNeeded();
                        groves[i].Detach();
                        groves[i] = null;
                    }
                }
            }

            //If all thugs are null it means gang war is over 
            int counter = 0; //to counter the number of null peds. if the number of null peds are as the "count" of the ballas&groves list then everyone from the team is dead
            for (int i = 0; i < groves.Count; i++)
            {
                if (ballas[i] == null && groves[i] == null)
                {
                    counter++;
                }
            }
            if (counter == groves.Count)
            {
                ballas = null;
                groves = null;
                eventActive = false;
                enterCarCounterG = new Dictionary<Ped, int>();
                enterCarCounterB = new Dictionary<Ped, int>();
                driveAwayCounterB = new Dictionary<Ped, int>();
                driveAwayCounterG = new Dictionary<Ped, int>();
                UI.ShowHelpMessage("GangWar over,either dead or ran away");
            }

            assignTask(ballas, groves); //I tried setting them in single method but it failed
        }

        /**
         * Used to delete thugs forcefully
         */
        public void deleteThugs()
        {
            if (ballas == null && groves == null)
            {
                return;
            }
            for (int i = 0; i < ballas.Count; i++)
            {
                if (ballas[i] != null)
                {
                    ballas[i].MarkAsNoLongerNeeded();
                    ballas[i].CurrentBlip.Remove();
                    ballas[i].Delete();
                    ballas[i] = null;
                }
            }
            for (int i = 0; i < groves.Count; i++)
            {
                if (groves[i] != null)
                {
                    groves[i].MarkAsNoLongerNeeded();
                    groves[i].CurrentBlip.Remove();
                    groves[i].Delete();
                    groves[i] = null;
                }
            }
            ballas = null;
            groves = null;
            eventActive = false;
        }

        /**
         * Returns a list of thugs who are still alive in the fight
         */
        private List<Ped> thugsalive(List<Ped> group)
        {
            if (group != null)
            {
                List<Ped> temp = new List<Ped>();
                foreach (Ped p in group)
                {
                    if (p != null && p.IsAlive)
                    {
                        temp.Add(p);
                    }
                }
                return temp;
            }
            else
                return null;
        }

        private bool safetyCheck()
        {
            if (ballas == null || groves == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void ifEnterCar(Ped p, Dictionary<Ped, int> enterCounter, int threshold)
        {
            //If you are in the nearest car as a driver then we will give u wanted 
            //If u enter as a passenger then u will get wanted and a waypoint.

            //TRY: Other cars will now follow the car u at
            startCHeck++;
            if (p.IsInCombat == false && p.IsGettingIntoAVehicle == false && p.IsTryingToEnterALockedVehicle == false && p.IsInVehicle() == false)
            {

                if (startCHeck > 80)
                {


                    Vehicle v = World.GetClosestVehicle(p.Position, 50000);

                    if (v == null)
                    {
                        //No car nearby. FUTURE PLANS
                    }

                    else
                    {
                        p.Task.EnterVehicle(v, VehicleSeat.Any);
                        enterCounter[p]++;
                        if (enterCounter[p] > threshold)
                        {
                            v = World.GetClosestVehicle(p.Position, 5000);
                            if (v == null)
                            {

                            }
                            else
                            {
                                p.Task.WarpIntoVehicle(v, VehicleSeat.Any);
                                enterCounter[p] = 0;
                            }
                        }
                    }
                    startCHeck = 80;
                }


            }
            else
            {
                enterCounter[p] = 0;
            }



        }
        public void shouldEnterCarCheck(int threshold)
        {
            if (!safetyCheck())
            {
                return;
            }
            assignTask(groves, ballas);

            List<Ped> aliveB = thugsalive(ballas);
            List<Ped> aliveG = thugsalive(groves);

            foreach (Ped p in aliveB)
            {
                ifEnterCar(p, enterCarCounterB, threshold);
            }
            foreach (Ped p in aliveG)
            {
                ifEnterCar(p, enterCarCounterG, threshold);
            }
        }

        private bool allThugIn(List<Ped> alive)
        {
            int counter = 0;
            if (alive != null)
            {
                foreach (Ped p in alive)
                {
                    if (p != null)
                    {
                        if (p.IsInVehicle() || p.IsSittingInVehicle())
                        {
                            counter++;
                        }
                    }
                }
                if (counter == alive.Count)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        private bool isPlayerInPassenger(Vehicle v)
        {
            if (v.IsSeatFree(VehicleSeat.RightFront) == false)
            {
                if (v.GetPedOnSeat(VehicleSeat.RightFront).Equals(Game.Player.Character))
                {
                    return true;
                }
            }
            if (v.IsSeatFree(VehicleSeat.RightRear) == false)
            {
                if (v.GetPedOnSeat(VehicleSeat.RightRear).Equals(Game.Player.Character))
                {
                    return true;
                }
            }
            if (v.IsSeatFree(VehicleSeat.LeftRear) == false)
            {
                if (v.GetPedOnSeat(VehicleSeat.LeftRear).Equals(Game.Player.Character))
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;

        }



        public void missionCheck()
        {
            if (missionActive)
            {
                int groupGrove = Function.Call<int>(Hash.GET_HASH_KEY, "AMBIENT_GANG_FAMILY");
                World.SetRelationshipBetweenGroups(Relationship.Companion, groupGrove, 0x6F0783F5);
                List<Ped> ped = thugsalive(groves);
                foreach (Ped p in ped)
                {
                    Vehicle v = p.CurrentVehicle;
                    if (v != null)
                    {
                        try
                        {
                            Ped driver = v.GetPedOnSeat(VehicleSeat.Driver);
                            driver.AlwaysKeepTask = true;
                            TaskSequence t = new TaskSequence();
                            v.Position = World.GetNextPositionOnStreet(Game.Player.Character.ForwardVector * -10);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }


        /*We need to make sure that all thugs who are in car DONT exit from their car anymore*
         * Need to make sure it is companion of player or else they will kill OR make them companion of player 
         * Need to make all the car wit thugs spawn behgind u and escort you
         * Disable the cruise around of gang and make them escort you instead
         * 
         * If player is dead end mission.
         * If car is destroyed mission ends.
         */
        private void playerIsDriver()
        {

            Blip blip = World.CreateBlip(new Vector3(1107f, -3157f, -37.51859f));
            blip.ShowRoute = true;
            UI.ShowSubtitle("Take your homies to the waypoint! Your other homies will follow you");
            missionActive = true;
            
        }
        private void playerIsPassenger()
        {
          //  missionActive = true;
        }
        private void driveConditions(Ped p, Dictionary<Ped, int> driveEncounter)
        {
            //This method is run insdie ifDrive method to check if the driver/passenger is a player or normal ped and assigns mission  basead on it

            Vehicle v = p.CurrentVehicle;
            if (v != null)
            {
                if (v.IsSeatFree(VehicleSeat.Driver)) //If no one in driver seat
                {

                    if (isPlayerInPassenger(v)) //If player is in passenger
                    {
                        //Create way point n etc (Another method)
                        playerIsPassenger();

                        p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                        v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                        driveEncounter[p] = 0;
                    }
                    else //If player is not in passenger
                    {
                        p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                        v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                        driveEncounter[p] = 0;
                    }
                }
                else //If Driver exists
                {
                    if (v.GetPedOnSeat(VehicleSeat.Driver).IsAlive == false || v.GetPedOnSeat(VehicleSeat.Driver).Exists() == false) //If driver is dead
                    {
                        if (isPlayerInPassenger(v))//If player is in passenger
                        {
                            //Create waypoiny and etc
                            playerIsPassenger();

                            v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                            driveEncounter[p] = 0;
                        }
                        else //player is not a ped
                        {
                            p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                            v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                            driveEncounter[p] = 0;
                        }

                    }
                    else //If driver is alive
                    {
                        if (v.GetPedOnSeat(VehicleSeat.Driver).Equals(Game.Player.Character)) //If driver is player
                        {
                            //Create a waypoint etc (method)
                            playerIsDriver();
                        }
                        else //If driver is not player
                        {
                            if (isPlayerInPassenger(v)) //If player is in passenger
                            {
                                //Create waypoint system etc
                                playerIsPassenger();

                                v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                driveEncounter[p] = 0;
                            }
                            else //if player is not in passenger
                            {
                                v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                driveEncounter[p] = 0;
                            }
                        }
                    }
                }
            }
        }
        private void ThugDriveCheck(Ped p, Dictionary<Ped, int> driveEncounter, int threshold, List<Ped> alive)
        {
            if (p.IsInVehicle() || p.IsSittingInVehicle())
            {

                Vehicle v = p.CurrentVehicle;
                driveEncounter[p]++;
                if (v == null)
                {
                    //.......
                }
                else
                {
                    //If threshold reached
                    if (driveEncounter[p] > threshold)
                    {
                        driveConditions(p, driveEncounter);
                    }
                    //All thugs are in
                    else if (allThugIn(alive)) //all thugs are in car
                    {
                        driveConditions(p, driveEncounter);
                    }
                    //seats are full
                    else if (!v.IsSeatFree(VehicleSeat.Any))
                    {
                        driveConditions(p, driveEncounter);
                    }
                    //If nothing has been triggered we increment counter
                    else
                    {
                        driveEncounter[p]++;
                    }

                }
            }
            else
            {
                driveEncounter[p] = 0;
            }
        }

        public void startDrivingCheck(int threshold)
        {
            if (!safetyCheck())
            {
                return;
            }
            List<Ped> aliveB = thugsalive(ballas);
            List<Ped> aliveG = thugsalive(groves);

            foreach (Ped p in aliveB)
            {
                ThugDriveCheck(p, driveAwayCounterB, threshold, aliveB);
            }
            foreach (Ped p in aliveG)
            {
                ThugDriveCheck(p, driveAwayCounterG, threshold, aliveG);
            }
        }

        public void blipUpdate()
        {
            if (safetyCheck() == false)
            {
                return;
            }
            foreach (Ped p in thugsalive(ballas))
            {
                if (p.IsInVehicle())
                {
                    p.CurrentBlip.Sprite = BlipSprite.GunCar;
                    p.CurrentBlip.Name = "Ballas in Car";

                }
                else
                {
                    p.CurrentBlip.Sprite = BlipSprite.Standard;
                    p.CurrentBlip.Color = BlipColor.Blue;
                    p.CurrentBlip.Name = "Ballas";
                }
            }
            foreach (Ped p in thugsalive(groves))
            {
                if (p.IsInVehicle())
                {
                    p.CurrentBlip.Sprite = BlipSprite.GetawayCar;
                    p.CurrentBlip.Name = "Grove in Car";
                }
                else
                {
                    p.CurrentBlip.Sprite = BlipSprite.Standard;
                    p.CurrentBlip.Color = BlipColor.Green;
                    p.CurrentBlip.Name = "Groves";
                }
            }
        }
    }
}