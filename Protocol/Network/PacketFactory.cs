using Protocol.Network.MinecraftPacket;

namespace Protocol.Network
{
	public static class PacketFactory
	{
		
		public static Packet translatePacket(int id, ReadOnlyMemory<byte> buffer, bool raknet = false)
		{
			if (raknet)
			{
				switch (id)
				{
					case 0x00:
						return new ConnectedPing().Decode(buffer);
					case 0x01:
						return new UnconnectedPing().Decode(buffer);
					case 0x03:
						return new ConnectedPong().Decode(buffer);
					case 0x04:
						return new DetectLostConnections().Decode(buffer);
					case 0x1c:
						return new UnconnectedPong().Decode(buffer);
					case 0x05:
						return new OpenConnectionRequest1().Decode(buffer);
					case 0x06:
						return new OpenConnectionReply1().Decode(buffer);
					case 0x07:
						return new OpenConnectionRequest2().Decode(buffer);
					case 0x08:
						return new OpenConnectionReply2().Decode(buffer);
					case 0x09:
						return new ConnectionRequest().Decode(buffer);
					case 0x10:
						return new ConnectionRequestAccepted().Decode(buffer);
					case 0x13:
						return new NewIncomingConnection().Decode(buffer);
					case 0x14:
						return new NoFreeIncomingConnections().Decode(buffer);
					case 0x15:
						return new DisconnectionNotification().Decode(buffer);
					case 0x17:
						return new ConnectionBanned().Decode(buffer);
					case 0x1A:
						return new IpRecentlyConnected().Decode(buffer);
					case 0xfe:
						return new McpeWrapper().Decode(buffer);
					default:
						return new UnknownPacket((byte)id, buffer);
				}
			}
			else
			{
				switch (id)
				{
					case 1:
						return new McpeLogin().Decode(buffer);
					case 2:
						return new McpePlayStatus().Decode(buffer);
					case 3:
						return new McpeServerToClientHandshake().Decode(buffer);
					case 4:
						return new McpeClientToServerHandshake().Decode(buffer);
					case 5:
						return new McpeDisconnect().Decode(buffer);
					case 6:
						return new McpeResourcePacksInfo().Decode(buffer);
					case 7:
						return new McpeResourcePackStack().Decode(buffer);
					case 8:
						return new McpeResourcePackClientResponse().Decode(buffer);
					case 9:
						return new McpeText().Decode(buffer);
					case 10:
						return new McpeSetTime().Decode(buffer);
					case 11:
						return new McpeStartGame().Decode(buffer);
					case 12:
						return new McpeAddPlayer().Decode(buffer);
					case 13:
						return new McpeAddEntity().Decode(buffer);
					case 14:
						return new McpeRemoveEntity().Decode(buffer);
					case 15:
						return new McpeAddItemEntity().Decode(buffer);

					case 17:
						return new McpeTakeItemEntity().Decode(buffer);
					case 18:
						return new McpeMoveEntity().Decode(buffer);
					case 19:
						return new McpeMovePlayer().Decode(buffer);

					case 21:
						return new McpeUpdateBlock().Decode(buffer);
					case 22:
						return new McpeAddPainting().Decode(buffer);


					case 25:
						return new McpeLevelEvent().Decode(buffer);
					case 26:
						return new McpeBlockEvent().Decode(buffer);
					case 27:
						return new McpeEntityEvent().Decode(buffer);
					case 28:
						return new McpeMobEffect().Decode(buffer);
					case 29:
						return new McpeUpdateAttributes().Decode(buffer);
					case 30:
						return new McpeInventoryTransaction().Decode(buffer);
					case 31:
						return new McpeMobEquipment().Decode(buffer);
					case 32:
						return new McpeMobArmorEquipment().Decode(buffer);
					case 33:
						return new McpeInteract().Decode(buffer);
					case 34:
						return new McpeBlockPickRequest().Decode(buffer);
					case 35:
						return new McpeEntityPickRequest().Decode(buffer);
					case 36:
						return new McpePlayerAction().Decode(buffer);

					case 38:
						return new McpeHurtArmor().Decode(buffer);
					case 39:
						return new McpeSetEntityData().Decode(buffer);
					case 40:
						return new McpeSetEntityMotion().Decode(buffer);
					case 41:
						return new McpeSetEntityLink().Decode(buffer);
					case 42:
						return new McpeSetHealth().Decode(buffer);
					case 43:
						return new McpeSetSpawnPosition().Decode(buffer);
					case 44:
						return new McpeAnimate().Decode(buffer);
					case 45:
						return new McpeRespawn().Decode(buffer);
					case 46:
						return new McpeContainerOpen().Decode(buffer);
					case 47:
						return new McpeContainerClose().Decode(buffer);
					case 48:
						return new McpePlayerHotbar().Decode(buffer);
					case 49:
						return new McpeInventoryContent().Decode(buffer);
					case 50:
						return new McpeInventorySlot().Decode(buffer);
					case 51:
						return new McpeContainerSetData().Decode(buffer);
					case 52:
						return new McpeCraftingData().Decode(buffer);

					case 54:
						return new McpeGuiDataPickItem().Decode(buffer);
					case 55:
						return new McpeAdventureSettings().Decode(buffer);
					case 56:
						return new McpeBlockEntityData().Decode(buffer);

					case 58:
						return new McpeLevelChunk().Decode(buffer);
					case 59:
						return new McpeSetCommandsEnabled().Decode(buffer);
					case 60:
						return new McpeSetDifficulty().Decode(buffer);
					case 61:
						return new McpeChangeDimension().Decode(buffer);
					case 62:
						return new McpeSetPlayerGameType().Decode(buffer);
					case 63:
						return new McpePlayerList().Decode(buffer);
					case 64:
						return new McpeSimpleEvent().Decode(buffer);
					case 65:
						return new McpeTelemetryEvent().Decode(buffer);
					case 66:
						return new McpeSpawnExperienceOrb().Decode(buffer);
					case 67:
						return new McpeClientboundMapItemData().Decode(buffer);
					case 68:
						return new McpeMapInfoRequest().Decode(buffer);
					case 69:
						return new McpeRequestChunkRadius().Decode(buffer);
					case 70:
						return new McpeChunkRadiusUpdate().Decode(buffer);

					case 72:
						return new McpeGameRulesChanged().Decode(buffer);
					case 73:
						return new McpeCamera().Decode(buffer);
					case 74:
						return new McpeBossEvent().Decode(buffer);
					case 75:
						return new McpeShowCredits().Decode(buffer);
					case 76:
						return new McpeAvailableCommands().Decode(buffer);
					case 77:
						return new McpeCommandRequest().Decode(buffer);
					case 78:
						return new McpeCommandBlockUpdate().Decode(buffer);
					case 79:
						return new McpeCommandOutput().Decode(buffer);
					case 80:
						return new McpeUpdateTrade().Decode(buffer);
					case 81:
						return new McpeUpdateEquipment().Decode(buffer);
					case 82:
						return new McpeResourcePackDataInfo().Decode(buffer);
					case 83:
						return new McpeResourcePackChunkData().Decode(buffer);
					case 84:
						return new McpeResourcePackChunkRequest().Decode(buffer);
					case 85:
						return new McpeTransfer().Decode(buffer);
					case 86:
						return new McpePlaySound().Decode(buffer);
					case 87:
						return new McpeStopSound().Decode(buffer);
					case 88:
						return new McpeSetTitle().Decode(buffer);
					case 89:
						return new McpeAddBehaviorTree().Decode(buffer);
					case 90:
						return new McpeStructureBlockUpdate().Decode(buffer);
					case 91:
						return new McpeShowStoreOffer().Decode(buffer);
					case 92:
						return new McpePurchaseReceipt().Decode(buffer);
					case 93:
						return new McpePlayerSkin().Decode(buffer);
					case 94:
						return new McpeSubClientLogin().Decode(buffer);
					case 95:
						return new McpeInitiateWebSocketConnection().Decode(buffer);
					case 96:
						return new McpeSetLastHurtBy().Decode(buffer);
					case 97:
						return new McpeBookEdit().Decode(buffer);
					case 98:
						return new McpeNpcRequest().Decode(buffer);
					case 99:
						return new McpePhotoTransfer().Decode(buffer);
					case 100:
						return new McpeModalFormRequest().Decode(buffer);
					case 101:
						return new McpeModalFormResponse().Decode(buffer);
					case 102:
						return new McpeServerSettingsRequest().Decode(buffer);
					case 103:
						return new McpeServerSettingsResponse().Decode(buffer);
					case 104:
						return new McpeShowProfile().Decode(buffer);
					case 105:
						return new McpeSetDefaultGameType().Decode(buffer);
					case 106:
						return new McpeRemoveObjective().Decode(buffer);
					case 107:
						return new McpeSetDisplayObjective().Decode(buffer);
					case 108:
						return new McpeSetScore().Decode(buffer);
					case 109:
						return new McpeLabTable().Decode(buffer);
					case 110:
						return new McpeUpdateBlockSynced().Decode(buffer);
					case 111:
						return new McpeMoveEntityDelta().Decode(buffer);
					case 112:
						return new McpeSetScoreboardIdentity().Decode(buffer);
					case 113:
						return new McpeSetLocalPlayerAsInitialized().Decode(buffer);
					case 114:
						return new McpeUpdateSoftEnum().Decode(buffer);
					case 115:
						return new McpeNetworkStackLatency().Decode(buffer);

					case 117:
						return new McpeScriptCustomEvent().Decode(buffer);
					case 118:
						return new McpeSpawnParticleEffect().Decode(buffer);
					case 119:
						return new McpeAvailableEntityIdentifiers().Decode(buffer);

					case 121:
						return new McpeNetworkChunkPublisherUpdate().Decode(buffer);
					case 122:
						return new McpeBiomeDefinitionList().Decode(buffer);
					case 123:
						return new McpeLevelSoundEvent().Decode(buffer);
					case 124:
						return new McpeLevelEventGeneric().Decode(buffer);
					case 125:
						return new McpeLecternUpdate().Decode(buffer);


					case 129:
						return new McpeClientCacheStatus().Decode(buffer);
					case 130:
						return new McpeOnScreenTextureAnimation().Decode(buffer);
					case 131:
						return new McpeMapCreateLockedCopy().Decode(buffer);
					case 132:
						return new McpeStructureTemplateDataExportRequest().Decode(buffer);
					case 133:
						return new McpeStructureTemplateDataExportResponse().Decode(buffer);

					case 135:
						return new McpeClientCacheBlobStatus().Decode(buffer);
					case 136:
						return new McpeClientCacheMissResponse().Decode(buffer);
					case 137:
						return new McpeEducationSettings().Decode(buffer);
					case 138:
						return new McpeEmotePacket().Decode(buffer);
					case 139:
						return new McpeMultiPlayerSettings().Decode(buffer);
					case 140:
						return new McpeSettingsCommand().Decode(buffer);
					case 141:
						return new McpeAnvilDamage().Decode(buffer);
					case 142:
						return new McpeCompletedUsingItem().Decode(buffer);
					case 143:
						return new McpeNetworkSettings().Decode(buffer);
					case 144:
						return new McpePlayerAuthInput().Decode(buffer);
					case 145:
						return new McpeCreativeContent().Decode(buffer);
					case 146:
						return new McpePlayerEnchantOptions().Decode(buffer);
					case 147:
						return new McpeItemStackRequest().Decode(buffer);
					case 148:
						return new McpeItemStackResponse().Decode(buffer);
					case 149:
						return new McpeHurtArmor().Decode(buffer);
					case 150:
						return new McpeCodeBuilder().Decode(buffer);
					case 151:
						return new McpeUpdatePlayerGameType().Decode(buffer);
					case 152:
						return new McpeEmoteList().Decode(buffer);
					case 153:
						return new McpePositionTrackingDBServerBroadcast().Decode(buffer);
					case 154:
						return new McpePositionTrackingDBClientRequest().Decode(buffer);
					case 155:
						return new McpeDebugInfo().Decode(buffer);

					case 156:
						return new McpePacketViolationWarning().Decode(buffer);
					case 157:
						return new McpeMotionPredictionHints().Decode(buffer);

					case 158:
						return new McpeAnimateEntity().Decode(buffer);

					case 159:
						return new McpeCamera().Decode(buffer);

					case 160:
						return new McpePlayerFog().Decode(buffer);
					case 161:
						return new McpeCorrectPlayerMovePrediction().Decode(buffer);

					case 162:
						return new McpeItemRegistry().Decode(buffer);

					case 163:
						return new McpeFilterTextPacket().Decode(buffer);
					case 164:
						return new McpeClientBoundDebugRenderer().Decode(buffer);
					case 165:
						return new McpeSyncEntityProperty().Decode(buffer);
					case 166:
						return new McpeAddVolumeEntity().Decode(buffer);

					case 167:
						return new McpeRemoveVolumeEntity().Decode(buffer);

					case 168:
						return new McpeSimulationType().Decode(buffer);

					case 169:
						return new McpeNPCDialogue().Decode(buffer);

					case 170:
						return new McpeEducationResourceURI().Decode(buffer);

					case 171:
						return new McpeCreatePhoto().Decode(buffer);

					case 172:
						return new McpeUpdateSubChunkBlocksPacket().Decode(buffer);
					case 173:
						return new McpePhotoInfoRequest().Decode(buffer);

					case 174:
						return new McpeSubChunkPacket().Decode(buffer);
					case 175:
						return new McpeSubChunkRequestPacket().Decode(buffer);
					case 176:
						return new McpeClientStartItemCooldown().Decode(buffer);

					case 177:
						return new McpeScriptMessage().Decode(buffer);

					case 178:
						return new McpeCodeBuilderSource().Decode(buffer);

					case 179:
						return new McpeTickingAreasLoadStatus().Decode(buffer);

					case 180:
						return new McpeDimensionData().Decode(buffer);
					case 181:
						return new McpeAgentAction().Decode(buffer);

					case 182:
						return new McpeChangeMobProperty().Decode(buffer);

					case 183:
						return new McpeLessonProgress().Decode(buffer);

					case 184:
						return new McpeRequestAbility().Decode(buffer);
					case 185:
						return new McpePermissionRequest().Decode(buffer);
					case 186:

					case 187:
						return new McpeUpdateAbilities().Decode(buffer);
					case 188:
						return new McpeUpdateAdventureSettings().Decode(buffer);
					case 189:
						return new McpeDeathInfo().Decode(buffer);

					case 190:
						return new McpeEditorNetwork().Decode(buffer);

					case 191:
						return new McpeFeatureRegistry().Decode(buffer);

					case 192:
						return new McpeServerStats().Decode(buffer);

					case 193:
						return new McpeRequestNetworkSettings().Decode(buffer);
					case 194:
						return new McpeGameTestRequest().Decode(buffer);

					case 195:
						return new McpeGameTestResults().Decode(buffer);

					case 196:
						return new McpeUpdateClientInputLocks().Decode(buffer);

					case 197:
						return new McpeClientCheatAbility().Decode(buffer);

					case 198:
						return new McpeCameraPresets().Decode(buffer);

					case 199:
						return new McpeUnlockedRecipes().Decode(buffer);

					case 300:
						return new McpeCameraInstruction().Decode(buffer);


					case 302:
						return new McpeTrimData().Decode(buffer);
					case 303:
						return new McpeOpenSign().Decode(buffer);
					case 304:
						return new McpeAlexEntityAnimation().Decode(buffer);
					case 305:
						return new McpeRefreshEntitlements().Decode(buffer);

					case 306:
						return new McpePlayerToggleCrafterSlotRequest().Decode(buffer);

					case 307:
						return new McpeSetInventoryOptions().Decode(buffer);
					case 308:
						return new McpeSetHud().Decode(buffer);

					case 309:
						return new McpeAwardAchievement().Decode(buffer);

					case 310:
						return new McpeClientBoundCloseForm().Decode(buffer);


					case 312:
						return new McpeServerboundLoadingScreen().Decode(buffer);
					case 313:
						return new McpeJigsawStructureData().Decode(buffer);

					case 314:
						return new McpeCurrentStructureFeature().Decode(buffer);

					case 315:
						return new McpeServerBoundDiagnostics().Decode(buffer);

					case 316:
						return new McpeCameraAimAssist().Decode(buffer);

					case 317:
						return new McpeContainerRegistryCleanup().Decode(buffer);

					case 318:
						return new McpeMovementEffect().Decode(buffer);


					case 320:
						return new McpeCameraAimAssistPresets().Decode(buffer);

					case 321:
						return new McpeClientCameraAimAssist().Decode(buffer);

					case 322:
						return new McpeClientMovementPredictionSync().Decode(buffer);

					case 323:
						return new McpeUpdateClientOptions().Decode(buffer);

					case 324:
						return new McpePlayerVideoCapture().Decode(buffer);

					case 325:
						return new McpePlayerUpdateEntityOverrides().Decode(buffer);

					case 326:
						return new McpePlayerLocation().Decode(buffer);

					case 327:
						return new McpeClientBoundControlSchemeSet().Decode(buffer);

					case 328:
						return new McbeDebugDrawer().Decode(buffer);
					case 329:
						return new McbeServerBoundPackSettingChange().Decode(buffer);
					case 331:
						return new McbeGraphicsOverrideParameter().Decode(buffer);
					default:
						return new UnknownPacket((byte)id, buffer);
				}
			}
		}
	}
}