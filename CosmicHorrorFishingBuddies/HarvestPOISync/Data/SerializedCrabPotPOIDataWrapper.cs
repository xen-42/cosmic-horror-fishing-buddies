namespace CosmicHorrorFishingBuddies.HarvestPOISync.Data
{
	internal class SerializedCrabPotPOIDataWrapper
	{
		public float yRotation;
		public float x;
		public float z;
		public string deployableItemId;
		public float durability;

		public SerializedCrabPotPOIDataWrapper() { }

		public SerializedCrabPotPOIDataWrapper(SerializedCrabPotPOIData data)
		{
			yRotation = data.yRotation;
			x = data.x;	
			z = data.z;
			deployableItemId = data.deployableItemId;
			durability = data.durability;
		}

		public SerializedCrabPotPOIData ToData()
		{
			var item = new SpatialItemInstance();
			item.id = deployableItemId;
			item.durability = durability;

			var crabPot = new SerializedCrabPotPOIData(item, x, z, yRotation);
			crabPot.Init();

			return crabPot;
		}
	}
}
