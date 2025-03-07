using System;
using System.IO;
using System.Windows;
using CUE4Parse.UE4.Versions;
using FModel.Settings;
using FModel.ViewModels;
using SkiaSharp;

namespace FModel.Creator;

public class Typefaces
{
    private readonly Uri _burbankBigCondensedBold = new("pack://application:,,,/Resources/BurbankBigCondensed-Bold.ttf");
    private const string Ext = ".ufont";

    // FortniteGame
    private const string FortniteBasePath = "FortniteGame/Plugins/FortUILibrary/Content/Fonts/";
    private const string AsiaErinm = "AsiaERINM"; // korean fortnite
    private const string BurbankBigCondensedBlack = "BurbankBigCondensed-Black"; // russian
    private const string BurbankBigRegularBlack = "BurbankBigRegular-Black";
    private const string BurbankBigRegularBold = "BurbankBigRegular-Bold"; // official fortnite ig
    private const string BurbankSmallMedium = "BurbankSmall-Medium";
    private const string DroidSansFortniteSubset = "DroidSans-Fortnite-Subset";
    private const string NotoColorEmoji = "NotoColorEmoji";
    private const string NotoSansBold = "NotoSans-Bold";
    private const string NotoSansFortniteBold = "NotoSans-Fortnite-Bold";
    private const string NotoSansFortniteItalic = "NotoSans-Fortnite-Italic";
    private const string NotoSansFortniteRegular = "NotoSans-Fortnite-Regular";
    private const string NotoSansItalic = "NotoSans-Italic";
    private const string NotoSansRegular = "NotoSans-Regular";
    private const string NotoSansArabicBlack = "NotoSansArabic-Black"; // arabic fortnite
    private const string NotoSansArabicBold = "NotoSansArabic-Bold";
    private const string NotoSansArabicRegular = "NotoSansArabic-Regular";
    private const string NotoSansJpBold = "NotoSansJP-Bold"; // japanese fortnite
    private const string NotoSansKrRegular = "NotoSansKR-Regular";
    private const string NotoSansScBlack = "NotoSansSC-Black"; // simplified chinese fortnite
    private const string NotoSansScRegular = "NotoSansSC-Regular";
    private const string NotoSansTcBlack = "NotoSansTC-Black"; // traditional chinese fortnite
    private const string NotoSansTcRegular = "NotoSansTC-Regular";
    private const string BurbankSmallBlack = "burbanksmall-black";
    private const string BurbankSmallBold = "burbanksmall-bold";

    // PandaGame
    private const string PandaGameBasePath = "/Game/Panda_Main/UI/Fonts/";
    private const string NormsStdCondensedExtraBoldItalic = "Norms/TT_Norms_Std_Condensed_ExtraBold_Italic";
    private const string NormsProExtraBoldItalic = "Norms/TT_Norms_Pro_ExtraBold_Italic";
    private const string NormsStdCondensedMedium = "Norms/TT_Norms_Std_Condensed_Medium";
    private const string XiangHeHeiScProBlack = "XiangHeHei_SC/MXiangHeHeiSCPro-Black";
    private const string XiangHeHeiScProHeavy = "XiangHeHei_SC/MXiangHeHeiSCPro-Heavy";

    private readonly CUE4ParseViewModel _viewModel;

    public readonly SKTypeface Default; // used as a fallback font for all untranslated strings (item source, ...)
    public readonly SKTypeface DisplayName;
    public readonly SKTypeface Description;
    public readonly SKTypeface Bottom; // must be null for non-latin base languages
    public readonly SKTypeface Bundle;
    public readonly SKTypeface BundleNumber;
    public readonly SKTypeface TandemDisplayName;
    public readonly SKTypeface TandemGenDescription;
    public readonly SKTypeface TandemAddDescription;

    public Typefaces(CUE4ParseViewModel viewModel)
    {
        _viewModel = viewModel;
        var language = UserSettings.Default.AssetLanguage;

        Default = SKTypeface.FromStream(Application.GetResourceStream(_burbankBigCondensedBold)?.Stream);

        switch (viewModel.Provider.ProjectName.ToUpperInvariant())
        {
            case "FORTNITEGAME":
                {
                    DisplayName = OnTheFly(FortniteBasePath +
                                           language switch
                                           {
                                               ELanguage.Korean => AsiaErinm,
                                               ELanguage.Russian => BurbankBigCondensedBlack,
                                               ELanguage.Japanese => NotoSansJpBold,
                                               ELanguage.Arabic => NotoSansArabicBlack,
                                               ELanguage.TraditionalChinese => NotoSansTcBlack,
                                               ELanguage.Chinese => NotoSansScBlack,
                                               _ => string.Empty
                                           } + Ext);

                    Description = OnTheFly(FortniteBasePath +
                                           language switch
                                           {
                                               ELanguage.Korean => NotoSansKrRegular,
                                               ELanguage.Japanese => NotoSansJpBold,
                                               ELanguage.Arabic => NotoSansArabicRegular,
                                               ELanguage.TraditionalChinese => NotoSansTcRegular,
                                               ELanguage.Chinese => NotoSansScRegular,
                                               _ => NotoSansRegular
                                           } + Ext);

                    Bottom = OnTheFly(FortniteBasePath +
                                      language switch
                                      {
                                          ELanguage.Korean => string.Empty,
                                          ELanguage.Japanese => string.Empty,
                                          ELanguage.Arabic => string.Empty,
                                          ELanguage.TraditionalChinese => string.Empty,
                                          ELanguage.Chinese => string.Empty,
                                          _ => BurbankSmallBold
                                      } + Ext, true);

                    BundleNumber = OnTheFly(FortniteBasePath + BurbankBigCondensedBlack + Ext);

                    Bundle = OnTheFly(FortniteBasePath +
                                      language switch
                                      {
                                          ELanguage.Korean => AsiaErinm,
                                          ELanguage.Russian => BurbankBigCondensedBlack,
                                          ELanguage.Japanese => NotoSansJpBold,
                                          ELanguage.Arabic => NotoSansArabicBlack,
                                          ELanguage.TraditionalChinese => NotoSansTcBlack,
                                          ELanguage.Chinese => NotoSansScBlack,
                                          _ => string.Empty
                                      } + Ext, true) ?? BundleNumber;

                    TandemDisplayName = OnTheFly(FortniteBasePath +
                                                 language switch
                                                 {
                                                     ELanguage.Korean => AsiaErinm,
                                                     ELanguage.Russian => BurbankBigCondensedBlack,
                                                     ELanguage.Japanese => NotoSansJpBold,
                                                     ELanguage.Arabic => NotoSansArabicBlack,
                                                     ELanguage.TraditionalChinese => NotoSansTcBlack,
                                                     ELanguage.Chinese => NotoSansScBlack,
                                                     _ => BurbankBigRegularBlack
                                                 } + Ext);

                    TandemGenDescription = OnTheFly(FortniteBasePath +
                                                    language switch
                                                    {
                                                        ELanguage.Korean => AsiaErinm,
                                                        ELanguage.Japanese => NotoSansJpBold,
                                                        ELanguage.Arabic => NotoSansArabicBlack,
                                                        ELanguage.TraditionalChinese => NotoSansTcBlack,
                                                        ELanguage.Chinese => NotoSansScBlack,
                                                        _ => BurbankSmallBlack
                                                    } + Ext);

                    TandemAddDescription = OnTheFly(FortniteBasePath +
                                                    language switch
                                                    {
                                                        ELanguage.Korean => AsiaErinm,
                                                        ELanguage.Japanese => NotoSansJpBold,
                                                        ELanguage.Arabic => NotoSansArabicBlack,
                                                        ELanguage.TraditionalChinese => NotoSansTcBlack,
                                                        ELanguage.Chinese => NotoSansScBlack,
                                                        _ => BurbankSmallBold
                                                    } + Ext);
                    break;
                }
            case "MULTIVERSUS":
                {
                    DisplayName = OnTheFly(PandaGameBasePath + language switch
                    {
                        ELanguage.Chinese => XiangHeHeiScProHeavy,
                        _ => NormsProExtraBoldItalic
                    } + Ext);

                    Description = OnTheFly(PandaGameBasePath + language switch
                    {
                        ELanguage.Chinese => XiangHeHeiScProBlack,
                        _ => NormsStdCondensedMedium
                    } + Ext);

                    TandemDisplayName = OnTheFly(PandaGameBasePath + language switch
                    {
                        ELanguage.Chinese => XiangHeHeiScProBlack,
                        _ => NormsStdCondensedExtraBoldItalic
                    } + Ext);

                    TandemGenDescription = OnTheFly(PandaGameBasePath + language switch
                    {
                        ELanguage.Chinese => XiangHeHeiScProHeavy,
                        _ => NormsStdCondensedMedium
                    } + Ext);
                    break;
                }
            default:
                {
                    DisplayName = Default;
                    Description = Default;
                    break;
                }
        }
    }

    public SKTypeface OnTheFly(string path, bool fallback = false)
    {
        if (!_viewModel.Provider.TrySaveAsset(path, out var data))
            return fallback ? null : Default;
        var m = new MemoryStream(data) { Position = 0 };
        return SKTypeface.FromStream(m);
    }
}
