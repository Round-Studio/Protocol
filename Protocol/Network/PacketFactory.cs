using System;
using Protocol.Network.MinecraftPacket;

namespace Protocol.Network
{
	public static class PacketFactory
	{
		
		public static Packet translatePacket(int id, ReadOnlyMemory<byte> buffer)
		{
				switch (id)
				{
					case 1:
						return new McbeLogin().SetBytes(buffer);
					case 2:
						return new McbePlayStatus().SetBytes(buffer);
					case 3:
						return new McbeServerToClientHandshake().SetBytes(buffer);
					case 4:
						return new McbeClientToServerHandshake().SetBytes(buffer);
					case 5:
						return new McbeDisconnect().SetBytes(buffer);
					case 6:
						return new McbeResourcePacksInfo().SetBytes(buffer);
					case 7:
						return new McbeResourcePackStack().SetBytes(buffer);
					case 8:
						return new McbeResourcePackClientResponse().SetBytes(buffer);
					case 9:
						return new McbeText().SetBytes(buffer);
					case 10:
						return new McbeSetTime().SetBytes(buffer);
					case 11:
						return new McbeStartGame().SetBytes(buffer);
					case 12:
						return new McbeAddPlayer().SetBytes(buffer);
					case 13:
						return new McbeAddEntity().SetBytes(buffer);
					case 14:
						return new McbeRemoveEntity().SetBytes(buffer);
					case 15:
						return new McbeAddItemEntity().SetBytes(buffer);

					case 17:
						return new McbeTakeItemEntity().SetBytes(buffer);
					case 18:
						return new McbeMoveEntity().SetBytes(buffer);
					case 19:
						return new McbeMovePlayer().SetBytes(buffer);

					case 21:
						return new McbeUpdateBlock().SetBytes(buffer);
					case 22:
						return new McbeAddPainting().SetBytes(buffer);

					case 23:
						return new McbeTickSync().SetBytes(buffer);

					case 25:
						return new McbeLevelEvent().SetBytes(buffer);
					case 26:
						return new McbeBlockEvent().SetBytes(buffer);
					case 27:
						return new McbeEntityEvent().SetBytes(buffer);
					case 28:
						return new McbeMobEffect().SetBytes(buffer);
					case 29:
						return new McbeUpdateAttributes().SetBytes(buffer);
					case 30:
						return new McbeInventoryTransaction().SetBytes(buffer);
					case 31:
						return new McbeMobEquipment().SetBytes(buffer);
					case 32:
						return new McbeMobArmorEquipment().SetBytes(buffer);
					case 33:
						return new McbeInteract().SetBytes(buffer);
					case 34:
						return new McbeBlockPickRequest().SetBytes(buffer);
					case 35:
						return new McbeEntityPickRequest().SetBytes(buffer);
					case 36:
						return new McbePlayerAction().SetBytes(buffer);

					case 38:
						return new McbeHurtArmor().SetBytes(buffer);
					case 39:
						return new McbeSetEntityData().SetBytes(buffer);
					case 40:
						return new McbeSetEntityMotion().SetBytes(buffer);
					case 41:
						return new McbeSetEntityLink().SetBytes(buffer);
					case 42:
						return new McbeSetHealth().SetBytes(buffer);
					case 43:
						return new McbeSetSpawnPosition().SetBytes(buffer);
					case 44:
						return new McbeAnimate().SetBytes(buffer);
					case 45:
						return new McbeRespawn().SetBytes(buffer);
					case 46:
						return new McbeContainerOpen().SetBytes(buffer);
					case 47:
						return new McbeContainerClose().SetBytes(buffer);
					case 48:
						return new McbePlayerHotbar().SetBytes(buffer);
					case 49:
						return new McbeInventoryContent().SetBytes(buffer);
					case 50:
						return new McbeInventorySlot().SetBytes(buffer);
					case 51:
						return new McbeContainerSetData().SetBytes(buffer);
					case 52:
						return new McbeCraftingData().SetBytes(buffer);

					case 54:
						return new McbeGuiDataPickItem().SetBytes(buffer);
					case 55:
						return new McbeAdventureSettings().SetBytes(buffer);
					case 56:
						return new McbeBlockEntityData().SetBytes(buffer);

					case 58:
						return new McbeLevelChunk().SetBytes(buffer);
					case 59:
						return new McbeSetCommandsEnabled().SetBytes(buffer);
					case 60:
						return new McbeSetDifficulty().SetBytes(buffer);
					case 61:
						return new McbeChangeDimension().SetBytes(buffer);
					case 62:
						return new McbeSetPlayerGameType().SetBytes(buffer);
					case 63:
						return new McbePlayerList().SetBytes(buffer);
					case 64:
						return new McbeSimpleEvent().SetBytes(buffer);
					case 65:
						return new McbeTelemetryEvent().SetBytes(buffer);
					case 66:
						return new McbeSpawnExperienceOrb().SetBytes(buffer);
					case 67:
						return new McbeClientboundMapItemData().SetBytes(buffer);
					case 68:
						return new McbeMapInfoRequest().SetBytes(buffer);
					case 69:
						return new McbeRequestChunkRadius().SetBytes(buffer);
					case 70:
						return new McbeChunkRadiusUpdate().SetBytes(buffer);

					case 72:
						return new McbeGameRulesChanged().SetBytes(buffer);
					case 73:
						return new McbeCamera().SetBytes(buffer);
					case 74:
						return new McbeBossEvent().SetBytes(buffer);
					case 75:
						return new McbeShowCredits().SetBytes(buffer);
					case 76:
						return new McbeAvailableCommands().SetBytes(buffer);
					case 77:
						return new McbeCommandRequest().SetBytes(buffer);
					case 78:
						return new McbeCommandBlockUpdate().SetBytes(buffer);
					case 79:
						return new McbeCommandOutput().SetBytes(buffer);
					case 80:
						return new McbeUpdateTrade().SetBytes(buffer);
					case 81:
						return new McbeUpdateEquipment().SetBytes(buffer);
					case 82:
						return new McbeResourcePackDataInfo().SetBytes(buffer);
					case 83:
						return new McbeResourcePackChunkData().SetBytes(buffer);
					case 84:
						return new McbeResourcePackChunkRequest().SetBytes(buffer);
					case 85:
						return new McbeTransfer().SetBytes(buffer);
					case 86:
						return new McbePlaySound().SetBytes(buffer);
					case 87:
						return new McbeStopSound().SetBytes(buffer);
					case 88:
						return new McbeSetTitle().SetBytes(buffer);
					case 89:
						return new McbeAddBehaviorTree().SetBytes(buffer);
					case 90:
						return new McbeStructureBlockUpdate().SetBytes(buffer);
					case 91:
						return new McbeShowStoreOffer().SetBytes(buffer);
					case 92:
						return new McbePurchaseReceipt().SetBytes(buffer);
					case 93:
						return new McbePlayerSkin().SetBytes(buffer);
					case 94:
						return new McbeSubClientLogin().SetBytes(buffer);
					case 95:
						return new McbeInitiateWebSocketConnection().SetBytes(buffer);
					case 96:
						return new McbeSetLastHurtBy().SetBytes(buffer);
					case 97:
						return new McbeBookEdit().SetBytes(buffer);
					case 98:
						return new McbeNpcRequest().SetBytes(buffer);
					case 99:
						return new McbePhotoTransfer().SetBytes(buffer);
					case 100:
						return new McbeModalFormRequest().SetBytes(buffer);
					case 101:
						return new McbeModalFormResponse().SetBytes(buffer);
					case 102:
						return new McbeServerSettingsRequest().SetBytes(buffer);
					case 103:
						return new McbeServerSettingsResponse().SetBytes(buffer);
					case 104:
						return new McbeShowProfile().SetBytes(buffer);
					case 105:
						return new McbeSetDefaultGameType().SetBytes(buffer);
					case 106:
						return new McbeRemoveObjective().SetBytes(buffer);
					case 107:
						return new McbeSetDisplayObjective().SetBytes(buffer);
					case 108:
						return new McbeSetScore().SetBytes(buffer);
					case 109:
						return new McbeLabTable().SetBytes(buffer);
					case 110:
						return new McbeUpdateBlockSynced().SetBytes(buffer);
					case 111:
						return new McbeMoveEntityDelta().SetBytes(buffer);
					case 112:
						return new McbeSetScoreboardIdentity().SetBytes(buffer);
					case 113:
						return new McbeSetLocalPlayerAsInitialized().SetBytes(buffer);
					case 114:
						return new McbeUpdateSoftEnum().SetBytes(buffer);
					case 115:
						return new McbeNetworkStackLatency().SetBytes(buffer);

					case 117:
						return new McbeScriptCustomEvent().SetBytes(buffer);
					case 118:
						return new McbeSpawnParticleEffect().SetBytes(buffer);
					case 119:
						return new McbeAvailableEntityIdentifiers().SetBytes(buffer);

					case 121:
						return new McbeNetworkChunkPublisherUpdate().SetBytes(buffer);
					case 122:
						return new McbeBiomeDefinitionList().SetBytes(buffer);
					case 123:
						return new McbeLevelSoundEvent().SetBytes(buffer);
					case 124:
						return new McbeLevelEventGeneric().SetBytes(buffer);
					case 125:
						return new McbeLecternUpdate().SetBytes(buffer);


					case 129:
						return new McbeClientCacheStatus().SetBytes(buffer);
					case 130:
						return new McbeOnScreenTextureAnimation().SetBytes(buffer);
					case 131:
						return new McbeMapCreateLockedCopy().SetBytes(buffer);
					case 132:
						return new McbeStructureTemplateDataExportRequest().SetBytes(buffer);
					case 133:
						return new McbeStructureTemplateDataExportResponse().SetBytes(buffer);

					case 135:
						return new McbeClientCacheBlobStatus().SetBytes(buffer);
					case 136:
						return new McbeClientCacheMissResponse().SetBytes(buffer);
					case 137:
						return new McbeEducationSettings().SetBytes(buffer);
					case 138:
						return new McbeEmotePacket().SetBytes(buffer);
					case 139:
						return new McbeMultiPlayerSettings().SetBytes(buffer);
					case 140:
						return new McbeSettingsCommand().SetBytes(buffer);
					case 141:
						return new McbeAnvilDamage().SetBytes(buffer);
					case 142:
						return new McbeCompletedUsingItem().SetBytes(buffer);
					case 143:
						return new McbeNetworkSettings().SetBytes(buffer);
					case 144:
						return new McbePlayerAuthInput().SetBytes(buffer);
					case 145:
						return new McbeCreativeContent().SetBytes(buffer);
					case 146:
						return new McbePlayerEnchantOptions().SetBytes(buffer);
					case 147:
						return new McbeItemStackRequest().SetBytes(buffer);
					case 148:
						return new McbeItemStackResponse().SetBytes(buffer);
					case 149:
						return new McbeHurtArmor().SetBytes(buffer);
					case 150:
						return new McbeCodeBuilder().SetBytes(buffer);
					case 151:
						return new McbeUpdatePlayerGameType().SetBytes(buffer);
					case 152:
						return new McbeEmoteList().SetBytes(buffer);
					case 153:
						return new McbePositionTrackingDBServerBroadcast().SetBytes(buffer);
					case 154:
						return new McbePositionTrackingDBClientRequest().SetBytes(buffer);
					case 155:
						return new McbeDebugInfo().SetBytes(buffer);

					case 156:
						return new McbePacketViolationWarning().SetBytes(buffer);
					case 157:
						return new McbeMotionPredictionHints().SetBytes(buffer);

					case 158:
						return new McbeAnimateEntity().SetBytes(buffer);

					case 159:
						return new McbeCamera().SetBytes(buffer);

					case 160:
						return new McbePlayerFog().SetBytes(buffer);
					case 161:
						return new McbeCorrectPlayerMovePrediction().SetBytes(buffer);

					case 162:
						return new McbeItemRegistry().SetBytes(buffer);

					case 163:
						return new McbeFilterTextPacket().SetBytes(buffer);
					case 164:
						return new McbeClientBoundDebugRenderer().SetBytes(buffer);
					case 165:
						return new McbeSyncEntityProperty().SetBytes(buffer);
					case 166:
						return new McbeAddVolumeEntity().SetBytes(buffer);

					case 167:
						return new McbeRemoveVolumeEntity().SetBytes(buffer);

					case 168:
						return new McbeSimulationType().SetBytes(buffer);

					case 169:
						return new McbeNPCDialogue().SetBytes(buffer);

					case 170:
						return new McbeEducationResourceURI().SetBytes(buffer);

					case 171:
						return new McbeCreatePhoto().SetBytes(buffer);

					case 172:
						return new McbeUpdateSubChunkBlocksPacket().SetBytes(buffer);
					case 173:
						return new McbePhotoInfoRequest().SetBytes(buffer);

					case 174:
						return new McbeSubChunkPacket().SetBytes(buffer);
					case 175:
						return new McbeSubChunkRequestPacket().SetBytes(buffer);
					case 176:
						return new McbeClientStartItemCooldown().SetBytes(buffer);

					case 177:
						return new McbeScriptMessage().SetBytes(buffer);

					case 178:
						return new McbeCodeBuilderSource().SetBytes(buffer);

					case 179:
						return new McbeTickingAreasLoadStatus().SetBytes(buffer);

					case 180:
						return new McbeDimensionData().SetBytes(buffer);
					case 181:
						return new McbeAgentAction().SetBytes(buffer);

					case 182:
						return new McbeChangeMobProperty().SetBytes(buffer);

					case 183:
						return new McbeLessonProgress().SetBytes(buffer);

					case 184:
						return new McbeRequestAbility().SetBytes(buffer);
					case 185:
						return new McbePermissionRequest().SetBytes(buffer);
					case 186:

					case 187:
						return new McbeUpdateAbilities().SetBytes(buffer);
					case 188:
						return new McbeUpdateAdventureSettings().SetBytes(buffer);
					case 189:
						return new McbeDeathInfo().SetBytes(buffer);

					case 190:
						return new McbeEditorNetwork().SetBytes(buffer);

					case 191:
						return new McbeFeatureRegistry().SetBytes(buffer);

					case 192:
						return new McbeServerStats().SetBytes(buffer);

					case 193:
						return new McbeRequestNetworkSettings().SetBytes(buffer);
					case 194:
						return new McbeGameTestRequest().SetBytes(buffer);

					case 195:
						return new McbeGameTestResults().SetBytes(buffer);

					case 196:
						return new McbeUpdateClientInputLocks().SetBytes(buffer);

					case 197:
						return new McbeClientCheatAbility().SetBytes(buffer);

					case 198:
						return new McbeCameraPresets().SetBytes(buffer);

					case 199:
						return new McbeUnlockedRecipes().SetBytes(buffer);

					case 300:
						return new McbeCameraInstruction().SetBytes(buffer);


					case 302:
						return new McbeTrimData().SetBytes(buffer);
					case 303:
						return new McbeOpenSign().SetBytes(buffer);
					case 304:
						return new McbeAlexEntityAnimation().SetBytes(buffer);
					case 305:
						return new McbeRefreshEntitlements().SetBytes(buffer);

					case 306:
						return new McbePlayerToggleCrafterSlotRequest().SetBytes(buffer);

					case 307:
						return new McbeSetInventoryOptions().SetBytes(buffer);
					case 308:
						return new McbeSetHud().SetBytes(buffer);

					case 309:
						return new McbeAwardAchievement().SetBytes(buffer);

					case 310:
						return new McbeClientBoundCloseForm().SetBytes(buffer);


					case 312:
						return new McbeServerboundLoadingScreen().SetBytes(buffer);
					case 313:
						return new McbeJigsawStructureData().SetBytes(buffer);

					case 314:
						return new McbeCurrentStructureFeature().SetBytes(buffer);

					case 315:
						return new McbeServerBoundDiagnostics().SetBytes(buffer);

					case 316:
						return new McbeCameraAimAssist().SetBytes(buffer);

					case 317:
						return new McbeContainerRegistryCleanup().SetBytes(buffer);

					case 318:
						return new McbeMovementEffect().SetBytes(buffer);


					case 320:
						return new McbeCameraAimAssistPresets().SetBytes(buffer);

					case 321:
						return new McbeClientCameraAimAssist().SetBytes(buffer);

					case 322:
						return new McbeClientMovementPredictionSync().SetBytes(buffer);

					case 323:
						return new McbeUpdateClientOptions().SetBytes(buffer);

					case 324:
						return new McbePlayerVideoCapture().SetBytes(buffer);

					case 325:
						return new McbePlayerUpdateEntityOverrides().SetBytes(buffer);

					case 326:
						return new McbePlayerLocation().SetBytes(buffer);

					case 327:
						return new McbeClientBoundControlSchemeSet().SetBytes(buffer);
					case 328:
						return new McbeDebugDrawer().SetBytes(buffer);
					case 329:
						return new McbeServerBoundPackSettingChange().SetBytes(buffer);
					case 331:
						return new McbeGraphicsOverrideParameter().SetBytes(buffer);
					case 333:
						return new McbeClientBoundDataDrivenUIShowScreen().SetBytes(buffer);
					case 334:
						return new McbeClientBoundDataDrivenUICloseAllScreens().SetBytes(buffer);
					case 335:
						return new McbeClientBoundDataDrivenUIReload().SetBytes(buffer);
					case 336:
						return new McbeClientBoundTextureShift().SetBytes(buffer);
					case 337:
						return new McbeVoxelShapes().SetBytes(buffer);
					case 338:
						return new McbeCameraSpline().SetBytes(buffer);
					case 339:
						return new McbeCameraAimAssistActorPriority().SetBytes(buffer);
					default:
						return new UnknownPacket((byte)id, buffer);
				}
			}
		}
	}