using System;
using System.Collections.Generic;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Components;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SeModDebugger.Thraxus.Common.BaseClasses;
using VRage;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Game.ObjectBuilders;
using VRage.ObjectBuilders;
using VRageMath;

namespace SeModDebugger.Thraxus.Mods
{
	public class AwwScrap : BaseLoggingClass
	{
		protected override string Id { get; } = nameof(AwwScrap);

		public const string ChatHandle = nameof(AwwScrap);

		private const string SpawnScrapBagCommand = "ssb";
		private const string ScrapSuffix = "Scrap";

		private readonly Dictionary<string, MyPhysicalItemDefinition> _scrapDictionary = new Dictionary<string, MyPhysicalItemDefinition>();
		private readonly Dictionary<string, Action> _chatActions;
		
		public AwwScrap()
		{
			SetupScrapDictionary();
			_chatActions = new Dictionary<string, Action>
			{
				{SpawnScrapBagCommand.ToLowerInvariant(),  SpawnScrapBag},
			};
		}

		private void SetupScrapDictionary()
		{
			foreach (var def in MyDefinitionManager.Static.GetDefinitionsOfType<MyPhysicalItemDefinition>())
			{
				if (!def.Public) continue;
				if (!ValidateScrap(def.Id.SubtypeName)) continue;
				_scrapDictionary.Add(def.Id.SubtypeName, def);
			}
		}

		public void HandleChatMessage(string message)
		{
			Action chatAction;
			if (!_chatActions.TryGetValue(message.ToLowerInvariant(), out chatAction)) return;
			chatAction?.Invoke();
			ShowOnScreenDebug(message);
		}

		private static void ShowOnScreenDebug(string message)
		{
			//(message, 5000, Color.Red.ToString());
			MyVisualScriptLogicProvider.AddNotificationLocal(message, MyFontEnum.Red, 5);
		}

		private void SpawnScrapBag()
		{
			MyInventory inventory = SpawnBodyBag();
			foreach (var sd in _scrapDictionary)
			{
				IMyInventoryItem invItem = new MyPhysicalInventoryItem()
				{
					Amount = 10,
					Content = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_Ore>(sd.Key)
				};
				inventory.Add(invItem, invItem.Amount);
			}
		}

		public override void Close()
		{
			base.Close();
		}

		private static bool ValidateScrap(string compName)
		{
			return compName.EndsWith(ScrapSuffix, StringComparison.OrdinalIgnoreCase) && !compName.Equals(ScrapSuffix, StringComparison.OrdinalIgnoreCase);
		}

		private MyInventory SpawnBodyBag()
		{   //MyEntityExtensions

			// Spawn below Char
			MatrixD worldMatrix = MyAPIGateway.Session.Player.Character.WorldMatrix;
			worldMatrix.Translation += worldMatrix.Backward + worldMatrix.Forward;

			var bagDefinition = new MyDefinitionId(typeof(MyObjectBuilder_InventoryBagEntity), "AstronautBackpack");
			MyContainerDefinition definition;

			if (!MyComponentContainerExtension.TryGetContainerDefinition(bagDefinition.TypeId, bagDefinition.SubtypeId, out definition))
				return null;

			MyEntity myEntity = MyEntities.CreateFromComponentContainerDefinitionAndAdd(definition.Id, fadeIn: false);

			var myInventoryBagEntity = myEntity as MyInventoryBagEntity;
			if (myInventoryBagEntity == null) return null;

			myInventoryBagEntity.OwnerIdentityId = 0;
			MyTimerComponent component;
			if (myInventoryBagEntity.Components.TryGet(out component))
			{
				component.ChangeTimerTick(5 * Common.Settings.TicksPerMinute);
			}

			myInventoryBagEntity.DisplayNameText = "Debug Goodie Bag!";

			myEntity.PositionComp.SetWorldMatrix(ref worldMatrix);
			myEntity.Physics.LinearVelocity = Vector3.Zero;
			myEntity.Physics.AngularVelocity = Vector3.Zero;
			myEntity.Render.EnableColorMaskHsv = true;
			//myEntity.Render.ColorMaskHsv = PlayerInventory.Owner.Render.ColorMaskHsv;
			myEntity.Render.ColorMaskHsv = new Vector3(1, 0, 1);

			var backpackInventory = new MyInventory((MyFixedPoint)100, 100000, new Vector3(5, 5, 5), MyInventoryFlags.CanSend)
			{
				RemoveEntityOnEmpty = true
			};

			myEntity.Components.Add((MyInventoryBase)backpackInventory);

			return myInventoryBagEntity.GetInventory();
		}
	}
}
