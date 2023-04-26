using Sandbox.Common.ObjectBuilders;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Game.ObjectBuilders.ComponentSystem;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace Digi.Examples
{
    // This object gets created and given to the entity type you specify in the attribute, optionally the subtype aswell.


    // It is not entirely reliable for some entity types, for example:
    //   - for grids it can not attach at all for MP clients;
    //   - characters it can sometimes not get added;
    //   - solar panels and oxygen farms overwrite their gamelogic comp so it breaks any mods trying to add to them;
    //   - and probably more...
    // Workaround for these is to use a session comp to track the entity additions&removals, storing them in a list/dictionary then update them yourself.


    // The MyEntityComponentDescriptor parameters:
    //
    // 1.  The typeof(MyObjectBuilder_BatteryBlock) represents the <TypeId>BatteryBlock</TypeId> from the SBC.
    //     Never use the OBs that end with "Definition" as those are not entities.
    //
    // 2.  Entity-controlled updates, always use false. For more info: https://forum.keenswh.com/threads/modapi-changes-jan-26.7392280/
    //
    // 3+. Subtype strings, you can add as many or few as you want.
    //     You can also remove them entirely if you want it to attach to all entities of that type regardless of subtype, like so:
    //         [MyEntityComponentDescriptor(typeof(MyObjectBuilder_BatteryBlock), false)]


    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_BatteryBlock), false)]
    public class Example_GameLogic : MyGameLogicComponent
    {
        private IMyCubeBlock block; // storing the entity as a block reference to avoid re-casting it every time it's needed, this is the lowest type a block entity can be.


        // NOTE: All methods are optional, I'm just presenting the options and you can remove any you don't actually need.

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            // the base methods are usually empty, except for OnAddedToContainer()'s, which has some sync stuff making it required to be called.
            base.Init(objectBuilder);

            // this method is called async! always do stuff in the first update instead.
            // unless you're sure it must be in this one (like initializing resource sink/source components would need to be here).

            // the objectBuilder arg is sometimes the serialized version of the entity.
            // it works for hand tools for example but not for blocks (probably because MyObjectBuilder_CubeBlock does not extend MyObjectBuilder_EntityBase).


            block = (IMyCubeBlock)Entity;


            // makes UpdateOnceBeforeFrame() execute.
            // this is a special flag that gets self-removed after the method is called.
            // it can be used multiple times but mind that there is overhead to setting this so avoid using it for continuous updates.
            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();

            if (block?.CubeGrid?.Physics == null) // ignore projected and other non-physical grids
                return;

            MyLog.Default.WriteLine("this nerd");
            // do stuff...
            // you can access things from session via Example_Session.Instance.[...]


            // in other places (session, terminal control callbacks, TSS, etc) where you have an entity and you want to get this gamelogic, you can use:
            //   ent.GameLogic?.GetAs<Example_GameLogic>()
            // which will simply return null if it's not found.
        }
    }
}