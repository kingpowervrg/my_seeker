using UnityEditor;
using UnityEngine;

namespace EngineCore.Editor
{
    public class TextureProcessor : AssetPostprocessor
    {
        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            TextureImporter importer = assetImporter as TextureImporter;


        }

        void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter importer = assetImporter as TextureImporter;
            string textureAssetPath = importer.assetPath;

            if (textureAssetPath.EndsWithFast(".exr"))
            {
                importer.textureType = TextureImporterType.Lightmap;
                importer.mipmapEnabled = true;
            }

            //处理拼图图片
            if (textureAssetPath.StartsWithFast("Assets/Res/JigsawTextures/"))
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePackingTag = string.Empty;
                importer.isReadable = true;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 256;

                TextureImporterPlatformSettings importerSettingForAndroid = new TextureImporterPlatformSettings();
                importerSettingForAndroid.overridden = true;
                importerSettingForAndroid.maxTextureSize = importer.maxTextureSize;
                importerSettingForAndroid.name = "Android";
                if (0 == texture.width % 4 && 0 == texture.height % 4)
                {
                    importerSettingForAndroid.format = TextureImporterFormat.RGBA16;
                }
                else
                    importerSettingForAndroid.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                importerSettingForAndroid.compressionQuality = 80;
                importer.SetPlatformTextureSettings(importerSettingForAndroid);

                TextureImporterPlatformSettings importerSettingForIOS = new TextureImporterPlatformSettings();
                importerSettingForIOS.overridden = true;
                importerSettingForIOS.maxTextureSize = importer.maxTextureSize;
                importerSettingForIOS.name = "iPhone";
                importerSettingForIOS.format = TextureImporterFormat.ASTC_RGBA_8x8;
                importerSettingForIOS.compressionQuality = 80;
                importer.SetPlatformTextureSettings(importerSettingForIOS);
            }

            //if (textureAssetPath.StartsWithFast("Assets/Res/Gui/UISprite"))
            //{
            //    TextureImporterPlatformSettings importerSettingForIOS = importer.GetPlatformTextureSettings("iPhone");
            //    if (importerSettingForIOS == null)
            //    {
            //        importerSettingForIOS = new TextureImporterPlatformSettings();
            //        importerSettingForIOS.maxTextureSize = importer.maxTextureSize;
            //        importerSettingForIOS.name = "iPhone";
            //        importerSettingForIOS.overridden = true;
            //    }

            //    if (importerSettingForIOS.format != TextureImporterFormat.ASTC_RGBA_6x6)
            //        importerSettingForIOS.format = TextureImporterFormat.ASTC_RGBA_6x6;

            //    importerSettingForIOS.compressionQuality = 80;

            //    importer.SetPlatformTextureSettings(importerSettingForIOS);
            //}

            AssetDatabase.Refresh();
        }

        void OnPostprocessModel(GameObject modelGameObject)
        {
            ModelImporter importer = assetImporter as ModelImporter;

        }
    }
}