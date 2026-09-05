using System.Text.Json;
using KitchenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Data;

public static class DbInitializer
{
    public static async Task SeedEcommerceDataAsync(AppDbContext context, ILogger logger)
    {
        // 1. Seed Categories if empty
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Id = "cat-countertops", Name = "Countertops", NameAr = "أسطح المطبخ (رخام وجرانيت)", Icon = "🪨", Slug = "countertops", DisplayOrder = 1 },
                new() { Id = "cat-cabinets", Name = "Cabinets & Units", NameAr = "دواليب وخزائن المطبخ", Icon = "🗄️", Slug = "cabinets", DisplayOrder = 2 },
                new() { Id = "cat-tiles", Name = "Tiles & Backsplash", NameAr = "سيراميك وبلاط الحائط", Icon = "🧱", Slug = "tiles", DisplayOrder = 3 },
                new() { Id = "cat-sinks", Name = "Sinks & Faucets", NameAr = "أحواض وخلاطات", Icon = "🚰", Slug = "sinks", DisplayOrder = 4 },
                new() { Id = "cat-hardware", Name = "Handles & Hardware", NameAr = "مقابض ومفصلات", Icon = "🔩", Slug = "hardware", DisplayOrder = 5 },
                new() { Id = "cat-lighting", Name = "Kitchen Lighting", NameAr = "إضاءة وليد المطبخ", Icon = "💡", Slug = "lighting", DisplayOrder = 6 },
                new() { Id = "cat-appliances", Name = "Built-in Appliances", NameAr = "أجهزة بلت إن", Icon = "🍳", Slug = "appliances", DisplayOrder = 7 },
                new() { Id = "cat-accessories", Name = "Storage & Organizers", NameAr = "منظمات وإكسسوارات", Icon = "🗃️", Slug = "accessories", DisplayOrder = 8 }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} product categories.", categories.Count);
        }

        // 2. Seed Promo Codes if empty
        if (!await context.PromoCodes.AnyAsync())
        {
            var promos = new List<PromoCode>
            {
                new() { Code = "KITCHEN10", DiscountPercent = 10, FixedDiscount = 0, MinOrder = 3000, Label = "10% Off Kitchen Materials", LabelAr = "خصم 10% للخامات", IsActive = true },
                new() { Code = "WELCOME20", DiscountPercent = 20, FixedDiscount = 0, MinOrder = 8000, Label = "20% VIP Kitchen Project", LabelAr = "خصم 20% للمشاريع", IsActive = true },
                new() { Code = "SAVE500", DiscountPercent = 0, FixedDiscount = 500, MinOrder = 2500, Label = "500 EGP Instant Discount", LabelAr = "خصم فوري 500 ج.م", IsActive = true }
            };

            await context.PromoCodes.AddRangeAsync(promos);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} promo codes.", promos.Count);
        }

        // 3. Seed Pickup Showrooms if empty
        if (!await context.Locations.AnyAsync())
        {
            var locations = new List<PickupLocation>
            {
                new()
                {
                    Id = "loc-1",
                    Name = "Aura Main Showroom — Zamalek",
                    NameAr = "معرض أورا الرئيسي — الزمالك",
                    Address = "26 26th of July St., Zamalek, Cairo",
                    AddressAr = "٢٦ شارع ٢٦ يوليو، الزمالك، القاهرة",
                    Hours = "Sat – Thu: 10:00 AM – 10:00 PM | Fri: 2:00 PM – 10:00 PM",
                    HoursAr = "السبت – الخميس: ١٠:٠٠ ص – ١٠:٠٠ م | الجمعة: ٢:٠٠ م – ١٠:٠٠ م",
                    Phone = "+20 100 555 0192",
                    StockStatus = "Samples & Consultation ready in 2 hours",
                    StockStatusAr = "العينات والاستشارة جاهزة خلال ساعتين",
                    IsDefault = true
                },
                new()
                {
                    Id = "loc-2",
                    Name = "Atelier Design Hub — Mall of Arabia",
                    NameAr = "مركز أورا للتصميم — مول العرب",
                    Address = "Gate 5, Luxury Wing, Sheikh Zayed, Giza",
                    AddressAr = "بوابة ٥، جناح الديكور، الشيخ زايد، الجيزة",
                    Hours = "Daily: 10:00 AM – 11:00 PM",
                    HoursAr = "يومياً: ١٠:٠٠ ص – ١١:٠٠ م",
                    Phone = "+20 100 555 0193",
                    StockStatus = "3D Simulation & Sample pickup",
                    StockStatusAr = "محاكاة 3D واستلام عينات الخامات",
                    IsDefault = false
                },
                new()
                {
                    Id = "loc-3",
                    Name = "Aura Fabrication & Stone Workshop — Maadi",
                    NameAr = "مصنع وورشة رخام أورا — المعادي",
                    Address = "Industrial Zone, Building 42, Degla, Maadi, Cairo",
                    AddressAr = "المنطقة الصناعية، مبنى ٤٢، دجلة، المعادي، القاهرة",
                    Hours = "Mon – Sat: 8:00 AM – 6:00 PM",
                    HoursAr = "الإثنين – السبت: ٨:٠٠ ص – ٦:٠٠ م",
                    Phone = "+20 100 555 0194",
                    StockStatus = "Direct workshop pickup & Slab inspection",
                    StockStatusAr = "استلام مباشر من المصنع ومعاينة الألواح",
                    IsDefault = false
                }
            };

            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} pickup showrooms.", locations.Count);
        }

        // 4. Seed Store Settings if empty
        if (!await context.Settings.AnyAsync())
        {
            var settings = new List<StoreSetting>
            {
                new() { Key = "storePhoneNumber", Value = "01005550192", Description = "WhatsApp Hotline" },
                new() { Key = "freeDeliveryThreshold", Value = "5000", Description = "Free shipping min cart value (EGP)" },
                new() { Key = "defaultDeliveryFee", Value = "100", Description = "Standard freight delivery fee (EGP)" },
                new() { Key = "instapayWalletNumber", Value = "01005550192", Description = "Instapay Transfer Phone Number" },
                new() { Key = "instapayAccountName", Value = "Aura Kitchens & Interiors", Description = "Account holder name" },
                new() { Key = "instapayBankName", Value = "CIB / National Bank of Egypt", Description = "Bank name" }
            };

            await context.Settings.AddRangeAsync(settings);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} store settings.", settings.Count);
        }

        // 5. Seed or Update Initial Products with Rich Attributes
        var existingProducts = await context.Products.ToListAsync();
        if (existingProducts.Count == 0)
        {
            var products = GetInitialProductsList();
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} initial kitchen products.", products.Count);
        }
        else
        {
            var initialDefs = GetInitialProductsList();
            bool updated = false;
            foreach (var existing in existingProducts)
            {
                var def = initialDefs.FirstOrDefault(p => p.Id == existing.Id);
                if (def != null && (string.IsNullOrEmpty(existing.SurfaceType) || existing.SurfaceType == "none"))
                {
                    existing.SurfaceType = def.SurfaceType;
                    existing.FeaturesJson = def.FeaturesJson;
                    existing.FeaturesArJson = def.FeaturesArJson;
                    updated = true;
                }
            }
            if (updated)
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Updated existing products with 3D surfaceTypes and bullet features.");
            }
        }
    }

    private static List<Product> GetInitialProductsList()
    {
        return new List<Product>
        {
                new()
                {
                    Id = "prod-1",
                    Name = "Black Galaxy Indian Granite Countertop",
                    NameAr = "سطح جرانيت بلاك جالاكسي هندي فاخر",
                    Price = 1450,
                    OriginalPrice = 1750,
                    UnitType = "per_sqm",
                    SurfaceType = "countertop",
                    CategoryId = "cat-countertops",
                    CategoryName = "Countertops",
                    CategoryNameAr = "أسطح المطبخ (رخام وجرانيت)",
                    Material = "Natural Granite",
                    MaterialAr = "جرانيت طبيعي صلب",
                    Finish = "Mirror Polished",
                    FinishAr = "تلميع كريستالي عالي",
                    Color = "Deep Obsidian & Golden Flecks",
                    ColorAr = "أسود داكن بنقاط ذهبية",
                    ColorHex = "#121217",
                    Thickness = "3.0 cm",
                    OriginCountry = "India",
                    OriginCountryAr = "الهند",
                    ImageUrl = "https://images.unsplash.com/photo-1600585152220-90363fe7e115?auto=format&fit=crop&q=80&w=1000",
                    VideoUrl = "/videos/video-4252302.mp4",
                    Badge = "Best Seller",
                    BadgeAr = "الأكثر طلباً",
                    IsFeatured = true,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 64,
                    ShortDescription = "High-density natural Indian granite with reflective gold speckles. Unmatched heat and scratch resistance for heavy-duty cooking.",
                    ShortDescriptionAr = "جرانيت هندي طبيعي عالي الكثافة مع نقاط ذهبية عاكسة للضوء. مقاومة مطلقة للحرارة والخدش مناسب للمطابخ النشطة.",
                    Description = "Black Galaxy is renowned worldwide for its deep cosmic obsidian base embedded with natural bronzite golden flecks. Quarried in Andhra Pradesh, India, each slab is diamond-polished to a mirror sheen. Highly impervious to thermal shock up to 350°C and knife scratches.",
                    DescriptionAr = "يعتبر جرانيت بلاك جالاكسي من أرقى أنواع الجرانيت الطبيعي في العالم. يتميز بلونه الأسود الملكي وتطعيماته الذهبية الطبيعية، ومقاومته للحرارة العالية حتى ٣٥٠ درجة مئوية والزيوت والخدوش.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "Heat shock resistance up to 350°C (Hot pots directly on slab)",
                        "Diamond mirror polishing with water-repellent sealer",
                        "Thickness: 3.0cm solid slab with chamfered or bullnose edges",
                        "25-Year warranty against thermal cracking and deep staining"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "مقاومة تامة للصدمات الحرارية حتى ٣٥٠ درجة مئوية",
                        "تلميع ألماسي مع طبقة حماية عازلة للسوائل والزيوت",
                        "سماكة ٣ سم مع معالجة حواف نصف دائرية أو شطف شياكة",
                        "ضمان ٢٥ عاماً ضد التشققات وتغير اللون"
                    })
                },
                new()
                {
                    Id = "prod-2",
                    Name = "Calacatta Gold Engineered Quartz",
                    NameAr = "سطح كوارتز كلاكتا جولد أسباني فاخر",
                    Price = 2150,
                    OriginalPrice = 2600,
                    UnitType = "per_sqm",
                    SurfaceType = "countertop",
                    CategoryId = "cat-countertops",
                    CategoryName = "Countertops",
                    CategoryNameAr = "أسطح المطبخ (رخام وجرانيت)",
                    Material = "Engineered Quartz (93% Natural Quartz)",
                    MaterialAr = "كوارتز معالج (٩٣٪ كوارتز طبيعي)",
                    Finish = "Silk Satin Touch",
                    FinishAr = "ملمس حريري مطفي",
                    Color = "Pure White & Golden Grey Veins",
                    ColorAr = "أبيض ناصع بعروق ذهبية ورمادية",
                    ColorHex = "#F4F2EB",
                    Thickness = "2.0 cm",
                    OriginCountry = "Spain",
                    OriginCountryAr = "أسبانيا",
                    ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?auto=format&fit=crop&q=80&w=1000",
                    VideoUrl = "/videos/video-4253136.mp4",
                    Badge = "Luxury Pick",
                    BadgeAr = "اختيار النخبة",
                    IsFeatured = true,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 5.0,
                    ReviewsCount = 48,
                    ShortDescription = "Spanish non-porous quartz featuring dramatic Calacatta veining. Completely stain-proof and hygienic.",
                    ShortDescriptionAr = "كوارتز أسباني غير مسامي يتميز بعروق كلاكتا الإيطالية الشهيرة. مانع للبقع بنسبة ١٠٠٪ ومضاد للبكتيريا.",
                    Description = "Engineered in Spain combining 93% pure quartz crystals with advanced antimicrobial polymers. Calacatta Gold brings the timeless elegance of Italian marble without the vulnerability to lemon acid, vinegar, or coffee stains.",
                    DescriptionAr = "يجمع بين الفخامة الإيطالية والمتانة التكنولوجية الأسبانية. سطح صلب غير مسامي يقاوم أصعب البقع (الليمون، القهوة، الشاي والخل) ولا يحتاج لأي صيانة دورية.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "0% Porosity: Anti-stain and certified food safe (NSF)",
                        "Elegant golden and smoky grey continuous vein patterns",
                        "Ultra-fine satin texture with easy wipe-down cleaning",
                        "15-Year manufacturer guarantee"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "مسامية صفرية: غير قابل لامتصاص البقع ومعتمد للمس الأطعمة",
                        "عروق متصلة وممتدة تضفي عمقاً وفخامة استثنائية للمطبخ",
                        "سطح أملس فائق النعومة وسهل المسح والتنظيف",
                        "ضمان شامل ١٥ سنة من الشركة المصنعة"
                    })
                },
                new()
                {
                    Id = "prod-3",
                    Name = "Glacier White Acrylic Corian Solid Surface",
                    NameAr = "سطح كوريان أكريليك صلب بدون فواصل",
                    Price = 2850,
                    OriginalPrice = 3300,
                    UnitType = "per_sqm",
                    SurfaceType = "countertop",
                    CategoryId = "cat-countertops",
                    CategoryName = "Countertops",
                    CategoryNameAr = "أسطح المطبخ (رخام وجرانيت)",
                    Material = "Acrylic Solid Surface",
                    MaterialAr = "كوريان أكريليك نقي",
                    Finish = "Seamless Velvet Matte",
                    FinishAr = "مطفي ناعم بدون أي لحامات ظاهرة",
                    Color = "Glacier White",
                    ColorAr = "أبيض جليدي نقي",
                    ColorHex = "#EDEDED",
                    Thickness = "1.2 cm + Substrate",
                    OriginCountry = "USA / Korea",
                    OriginCountryAr = "أمريكي / كوري",
                    ImageUrl = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Seamless",
                    BadgeAr = "بدون لحام",
                    IsFeatured = false,
                    IsBestSeller = false,
                    InStock = true,
                    Rating = 4.8,
                    ReviewsCount = 31,
                    ShortDescription = "100% Seamless thermoformable solid surface. Allows integrated sinks, curved backsplashes, and zero dirt traps.",
                    ShortDescriptionAr = "سطح كوريان صلب يُركب بدون أي فواصل أو لحامات مرئية. يدعم دمج الحوض كقطعة واحدة وقابل للتشكيل والترميم.",
                    Description = "Thermoformable acrylic composite engineered for modern seamless kitchens. Sinks and backsplashes can be molded seamlessly with the countertop, leaving zero crevices for mold or bacteria to accumulate.",
                    DescriptionAr = "يتميز بإمكانية تشكيل حوض المطبخ والوزرة الخلفية ككتلة واحدة مدمجة بالكامل، مع إمكانية صنفرته وتلميعه وإعادته كالجديد تماماً عند حدوث أي خدش.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "Inconspicuous seamless bonding technology",
                        "Integrated under-mount seamless sink capability",
                        "Easily renewable and polishable on-site",
                        "Impervious to water, fungus, and bacteria growth"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "تقنية دمج حراري تخفي أي خطوط لحام تماماً",
                        "إمكانية تصنيع حوض مطبخ مدمج بدون حواف متراكمة للأوساخ",
                        "قابل للصنفرة والتجديد في مكانه ليعود كالجديد",
                        "مقاوم للرطوبة والعفن والبكتيريا بنسبة ١٠٠٪"
                    })
                },
                new()
                {
                    Id = "prod-4",
                    Name = "High-Gloss Pure White PVC Kitchen Cabinets",
                    NameAr = "دواليب مطبخ PVC أبيض لامع فائق المقاومة للماء",
                    Price = 3600,
                    OriginalPrice = 4200,
                    UnitType = "per_linear_meter",
                    SurfaceType = "cabinet",
                    CategoryId = "cat-cabinets",
                    CategoryName = "Cabinets & Units",
                    CategoryNameAr = "دواليب وخزائن المطبخ",
                    Material = "Thermal PVC Membrane on German MDF",
                    MaterialAr = "بي في سي حراري على MDF ألماني مقاوم للرطوبة",
                    Finish = "Ultra High Gloss Reflective",
                    FinishAr = "لامع عاكس للضوء سهل التنظيف",
                    Color = "Pure Arctic White",
                    ColorAr = "أبيض ناصع لامع",
                    ColorHex = "#FFFFFF",
                    Thickness = "18 mm",
                    OriginCountry = "Germany & Egypt Assembly",
                    OriginCountryAr = "خامات ألمانية وتجميع مصانعنا بمصر",
                    ImageUrl = "https://images.unsplash.com/photo-1556909114-44e3e70034e2?auto=format&fit=crop&q=80&w=1000",
                    VideoUrl = "/videos/video-34208848.mp4",
                    Badge = "Popular",
                    BadgeAr = "شائع ومفضل",
                    IsFeatured = true,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 4.8,
                    ReviewsCount = 92,
                    ShortDescription = "Modern waterproof PVC wrapped cabinetry with soft-close German hinges and aluminum protective kickplates.",
                    ShortDescriptionAr = "دواليب مطبخ عصرية مغلفة بالـ PVC الحراري المقاوم للماء والبخار مع مفصلات ألمانية هيدروليك تغلق بسلاسة.",
                    Description = "Engineered specifically for humid kitchen environments. The German green moisture-resistant MDF core is wrapped in a seamless thermo-bonded PVC membrane that prevents peeling and moisture penetration.",
                    DescriptionAr = "مصممة لتحمل الرطوبة العالية وأبخرة الطهي. شاسيه داخلي قوي من الـ MDF المعالج باللون الأخضر المقاوم للماء مع طبقة بي في سي خارجية عاكسة للضوء تكبر مساحة المطبخ بصرياً.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "100% Water-resistant thermal membrane vacuum press",
                        "Blum / Samet hydraulic soft-close hinges included",
                        "Waterproof bottom PVC legs with aluminum plinth",
                        "10-Year warranty against door warping and peeling"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "كبس حراري بالفراغ يمنع تسرب المياه تماماً",
                        "تشمل مفصلات بلوم / ساميت هيدروليك إغلاق ناعم",
                        "أرجل مقاومة للمياه مع وزرة ألومنيوم سفلية أنيقة",
                        "ضمان ١٠ سنوات ضد التقشير وتقوس الدرف"
                    })
                },
                new()
                {
                    Id = "prod-5",
                    Name = "Super-Matte Anthracite Grey Acrylic Cabinets",
                    NameAr = "دواليب مطبخ أكريليك رمادي مطفي مانع للبصمات",
                    Price = 5400,
                    OriginalPrice = 6200,
                    UnitType = "per_linear_meter",
                    SurfaceType = "cabinet",
                    CategoryId = "cat-cabinets",
                    CategoryName = "Cabinets & Units",
                    CategoryNameAr = "دواليب وخزائن المطبخ",
                    Material = "2mm Senosan Pure Acrylic Layer",
                    MaterialAr = "طبقة أكريليك سينوسان نمساوي ٢ مم",
                    Finish = "Anti-Fingerprint Velvety Matte",
                    FinishAr = "مطفي مخملي مقاوم للبصمات والخدش",
                    Color = "Anthracite Dark Graphite",
                    ColorAr = "رمادي فحمي أنيق",
                    ColorHex = "#3A3B3C",
                    Thickness = "18.5 mm",
                    OriginCountry = "Austria",
                    OriginCountryAr = "النمسا",
                    ImageUrl = "https://images.unsplash.com/photo-1600573472592-401b489a3cdc?auto=format&fit=crop&q=80&w=1000",
                    VideoUrl = "/videos/video-5823681.mp4",
                    Badge = "Modern Luxury",
                    BadgeAr = "فخامة مودرن",
                    IsFeatured = true,
                    IsBestSeller = false,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 39,
                    ShortDescription = "Austrian Senosan acrylic panels with nanotech anti-fingerprint coating and laser-sealed edgebanding.",
                    ShortDescriptionAr = "ألواح أكريليك سينوسان نمساوية بتقنية النانو المانعة للبصمات مع شريط حواف ليزري خفي بدون أي خطوط غراء.",
                    Description = "The pinnacle of contemporary European kitchen architecture. Featuring authentic Austrian Senosan acrylic with micro-textured matte surface that eliminates fingerprints and reflects ambient lighting gracefully.",
                    DescriptionAr = "قمة التصميم الأوروبي المعاصر للمطابخ المودرن الفاخرة. ملمس حريري دافئ لا يترك أثراً لبصمات الأصابع مع قفل حواف ليزر مضاد للمياه والحرارة.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "Nanotechnology anti-fingerprint surface protection",
                        "Zero-joint laser edge banding: 100% moisture barrier",
                        "UV resistant: Color will not fade or yellow over time",
                        "12-Year comprehensive warranty"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "تقنية نانو تحمي السطح من بصمات الأصابع والدهون",
                        "لحام حواف بالليزر بدون أي فواصل غراء",
                        "مقاومة فائقة لأشعة الشمس وثبات تام للون",
                        "ضمان شامل ١٢ عاماً"
                    })
                },
                new()
                {
                    Id = "prod-6",
                    Name = "Natural Oak HPL Textured Cabinets",
                    NameAr = "دواليب مطبخ HPL بنقشة خشب الأرو الطبيعي",
                    Price = 3850,
                    OriginalPrice = 4400,
                    UnitType = "per_linear_meter",
                    SurfaceType = "cabinet",
                    CategoryId = "cat-cabinets",
                    CategoryName = "Cabinets & Units",
                    CategoryNameAr = "دواليب وخزائن المطبخ",
                    Material = "High Pressure Laminate (HPL) on HMR Core",
                    MaterialAr = "إتش بي إل عالي الضغط مقاوم للخدش والرطوبة",
                    Finish = "Synchronized Woodgrain Embossed",
                    FinishAr = "تجسيم خشبي بارز يماثل الخشب الطبيعي",
                    Color = "Warm Scandinavian Oak",
                    ColorAr = "خشب أرو اسكندنافي دافئ",
                    ColorHex = "#9E7A4A",
                    Thickness = "18 mm",
                    OriginCountry = "Italy & Turkey",
                    OriginCountryAr = "إيطاليا وتركيا",
                    ImageUrl = "https://images.unsplash.com/photo-1600210491369-e753d80a41f3?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Warm Aesthetic",
                    BadgeAr = "طابع خشبي دافئ",
                    IsFeatured = false,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 4.7,
                    ReviewsCount = 54,
                    ShortDescription = "High-pressure laminate with realistic embossed wood grain. Ultra-scratch proof and heat resistant.",
                    ShortDescriptionAr = "خامة HPL الإيطالية المضغوطة بمظهر وملمس الخشب الطبيعي الدافئ، شديدة المقاومة للخدوش والأحماض والحرارة.",
                    Description = "Combines the cozy biophilic warmth of organic wood with the unmatched structural resilience of thermosetting HPL resin laminates. Perfect for Scandinavian and modern rustic kitchens.",
                    DescriptionAr = "يمنحك دفء وجمال الخشب الطبيعي دون القلق من تمدد الخشب أو الرطوبة أو الحشرات. طبقة HPL متينة تقاوم الخدوش وسهلة المسح.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "Extreme scratch and impact resistance (HPL grade)",
                        "Warm tactile woodgrain texture that resists kitchen grease",
                        "Environmentally certified low-emission core",
                        "10-Year warranty"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "مقاومة استثنائية للاحتكاك والصدمات وسكاكين المطبخ",
                        "نقشة خشبية مريحة للعين ومقاومة لزيوت الطهي",
                        "شاسيه داخلي معالج وصديق للبيئة",
                        "ضمان ١٠ سنوات"
                    })
                },
                new()
                {
                    Id = "prod-7",
                    Name = "Glossy White Beveled Metro Subway Tiles",
                    NameAr = "بلاط مترو صب واي أبيض لامع مشطوف",
                    Price = 240,
                    OriginalPrice = 310,
                    UnitType = "per_sqm",
                    SurfaceType = "backsplash",
                    CategoryId = "cat-tiles",
                    CategoryName = "Tiles & Backsplash",
                    CategoryNameAr = "سيراميك وبلاط الحائط",
                    Material = "Glazed Ceramic Tile (10x20 cm)",
                    MaterialAr = "سيراميك مطلي عالي الجودة (١٠×٢٠ سم)",
                    Finish = "Beveled Edge Glossy Glaze",
                    FinishAr = "حواف مشطوفة ثلاثية الأبعاد بلمعان كريستالي",
                    Color = "Crisp White",
                    ColorAr = "أبيض ناصع كلاسيكي",
                    ColorHex = "#FAFAFA",
                    Thickness = "8 mm",
                    OriginCountry = "Egypt / Spain",
                    OriginCountryAr = "مصر / أسبانيا",
                    ImageUrl = "https://images.unsplash.com/photo-1600566752355-35792bedcfea?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Timeless Classic",
                    BadgeAr = "كلاسيكي خالد",
                    IsFeatured = true,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 110,
                    ShortDescription = "Iconic beveled subway tiles for kitchen backsplashes. Effortless to wipe clean of cooking splatter.",
                    ShortDescriptionAr = "بلاط المترو الكلاسيكي المشهور لحوائط المطبخ وخلف البوتاجاز. سهل التنظيف من بقع الزيوت ويمنح إضاءة رائعة.",
                    Description = "The definitive architectural choice for kitchen splashbacks. The 3D beveled profile bounces light throughout the kitchen, while the vitrified glaze prevents grease and spice stains from adhering.",
                    DescriptionAr = "البلاط المفضل لأشهر مصممي الديكور عالمياً. يعكس الإضاءة بشكل رائع ويجعل منطقة العمل فوق أسطح المطبخ مشرقة ونظيفة دوماً.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "Stain-proof vitrified ceramic glaze (Wipes clean with damp cloth)",
                        "Classic 10x20 cm beveled brick dimension",
                        "Resistant to stove heat and hot oil splatters",
                        "Packaging: 1.0 m² per box"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "طبقة زجاجية عازلة تمسح بقطعة قماش في ثوانٍ",
                        "مقاس كلاسيكي ١٠×٢٠ سم بشطف حواف أنيق",
                        "مقاوم لحرارة عيون البوتاجاز وتطاير الزيوت الساخنة",
                        "التعبئة: الكرتونة تحتوي على ١ متر مربع"
                    })
                },
                new()
                {
                    Id = "prod-8",
                    Name = "Moroccan Handcrafted Emerald Zellige Tiles",
                    NameAr = "بلاط زليج مغربي يدوي زمردي ملكي",
                    Price = 780,
                    OriginalPrice = 950,
                    UnitType = "per_sqm",
                    SurfaceType = "backsplash",
                    CategoryId = "cat-tiles",
                    CategoryName = "Tiles & Backsplash",
                    CategoryNameAr = "سيراميك وبلاط الحائط",
                    Material = "Artisanal Clay & Mineral Glaze",
                    MaterialAr = "طين طبيعي مفخور يدوياً مع ألوان معدنية",
                    Finish = "Organic Hand-Cut Glossy Variations",
                    FinishAr = "تدرجات لونية يدوية فريدة ولمعان حيوي",
                    Color = "Deep Emerald & Forest Green",
                    ColorAr = "أخضر زمردي ملكي وتدرجات زيتية",
                    ColorHex = "#1E4D38",
                    Thickness = "12 mm",
                    OriginCountry = "Morocco (Fez Artisans)",
                    OriginCountryAr = "المغرب (صناع فاس الأصليين)",
                    ImageUrl = "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Handcrafted",
                    BadgeAr = "صناعة يدوية",
                    IsFeatured = true,
                    IsBestSeller = false,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 36,
                    ShortDescription = "Authentic handcrafted Moroccan Zellige tiles. Each piece features natural tonal shifts and charming artisanal irregularities.",
                    ShortDescriptionAr = "بلاط زليج مغربي أصلي مصنوع يدوياً من طين فاس. يتميز بتموجات لونية طبيعية تمنح المطبخ روحاً تراثية فاخرة.",
                    Description = "Hand-chiseled by master artisans in Fez using methods unchanged for centuries. The jewel-toned green mineral glaze reflects light with breathtaking depth and handmade warmth.",
                    DescriptionAr = "قطعة فنية تزين جدران مطبخك. كل بلاطة فريدة في تموجاتها اللمعانية وانعكاساتها مع الإضاءة الليد تحت الدواليب.",
                    FeaturesJson = JsonSerializer.Serialize(new[] {
                        "Authentic 10x10 cm hand-chiseled Moroccan clay tile",
                        "Rich emerald green natural mineral glaze",
                        "Unique bespoke aesthetic impossible to replicate by machines",
                        "Heat proof and permanent color guarantee"
                    }),
                    FeaturesArJson = JsonSerializer.Serialize(new[] {
                        "مقاس ١٠×١٠ سم مقطوع ومفخور بالطرق التقليدية",
                        "لون أخضر زمردي عميق بصبغات معدنية طبيعية",
                        "طابع ديكوري فريد لا يمكن للماكينات محاكاته",
                        "مقاوم للحرارة وثبات تام للألوان مدى الحياة"
                    })
                },
                new()
                {
                    Id = "prod-9",
                    Name = "Franke Black Granite Composite Double Sink",
                    NameAr = "حوض فرانكي جرانيت مزدوج أسود غير قابل للخدش",
                    Price = 6200,
                    OriginalPrice = 7500,
                    UnitType = "per_piece",
                    SurfaceType = "accessory",
                    CategoryId = "cat-sinks",
                    CategoryName = "Sinks & Faucets",
                    CategoryNameAr = "أحواض وخلاطات",
                    Material = "Fragranite (80% Quartz Granite Composite)",
                    MaterialAr = "فراجرانيت (٨٠٪ جرانيت كوارتز ألماني)",
                    Finish = "Velvety Matte Antibacterial",
                    FinishAr = "مطفي مضاد للبكتيريا والخدوش",
                    Color = "Onyx Matte Black",
                    ColorAr = "أسود أونيكس ملكي",
                    ColorHex = "#1A1A1A",
                    Thickness = "10 mm Wall",
                    OriginCountry = "Germany / Switzerland",
                    OriginCountryAr = "سويسرا / ألمانيا",
                    ImageUrl = "https://images.unsplash.com/photo-1584622650111-993a426fbf0a?auto=format&fit=crop&q=80&w=1000",
                    Badge = "German Tech",
                    BadgeAr = "تكنولوجيا ألمانية",
                    IsFeatured = true,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 5.0,
                    ReviewsCount = 42,
                    ShortDescription = "Franke Fragranite double-bowl sink with Sanitized silver ion protection. Thermal shock proof up to 280°C.",
                    ShortDescriptionAr = "حوض فرانكي الألماني حلتين مصنوع من خامة الفراجرانيت المقاومة للكسر والخدش والحرارة مع تقنية الحماية الفضية من البكتيريا.",
                    Description = "Crafted with 80% natural quartz sand bonded with high-grade acrylic resins. Treated with Sanitized technology that reduces bacterial growth by 99%.",
                    DescriptionAr = "حوض فاخر يتحدى الزمن بمقاومته الفائقة لضربات الأواني الساخنة والسكاكين الحادة دون أي أثر أو بقع كلسية."
                },
                new()
                {
                    Id = "prod-10",
                    Name = "Commercial Pull-Out Chef Kitchen Faucet",
                    NameAr = "خلاط مطبخ شيف احترافي سوستة سحب ستانلس",
                    Price = 2750,
                    OriginalPrice = 3400,
                    UnitType = "per_piece",
                    SurfaceType = "accessory",
                    CategoryId = "cat-sinks",
                    CategoryName = "Sinks & Faucets",
                    CategoryNameAr = "أحواض وخلاطات",
                    Material = "Solid SUS304 Stainless Steel & Brass Core",
                    MaterialAr = "ستانلس ستيل SUS304 نقي وقلب نحاسي سيراميك",
                    Finish = "Brushed Titanium PVD",
                    FinishAr = "تيتانيوم مصقول بتقنية PVD المضادة للتكلس",
                    Color = "Brushed Gunmetal Charcoal",
                    ColorAr = "رمادي جون ميتال فاخر",
                    ColorHex = "#4A4D52",
                    Thickness = "Heavy Cast",
                    OriginCountry = "Italy",
                    OriginCountryAr = "إيطاليا",
                    ImageUrl = "https://images.unsplash.com/photo-1584622781564-1d987f7333c1?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Dual Spray",
                    BadgeAr = "دش وشلال",
                    IsFeatured = false,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 4.8,
                    ReviewsCount = 29,
                    ShortDescription = "360° Swivel commercial-grade spring faucet with magnetic docking and dual-flow shower spray mode.",
                    ShortDescriptionAr = "خلاط مطبخ احترافي بذراع سحب مرن ورشاش شلال مزدوج، دوران ٣٦٠ درجة لغسيل أواني المطبخ الكبيرة بسهولة.",
                    Description = "Engineered for home gourmet chefs. The flexible braided stainless spring delivers high water pressure, while Sedal ceramic cartridge ensures 500,000 drip-free uses.",
                    DescriptionAr = "مزود بخرطوشة سيراميك إيطالية تضمن عدم تسريب قطرة ماء واحدة لعشرات السنين مع تحكم سلس في قوة وضغط المياه."
                },
                new()
                {
                    Id = "prod-11",
                    Name = "Brushed Brass Knurled T-Bar Cabinet Handles",
                    NameAr = "طقم مقابض دواليب مطبخ نحاس محبب مودرن فاخر",
                    Price = 160,
                    OriginalPrice = 210,
                    UnitType = "per_piece",
                    SurfaceType = "accessory",
                    CategoryId = "cat-hardware",
                    CategoryName = "Handles & Hardware",
                    CategoryNameAr = "مقابض ومفصلات",
                    Material = "Solid Solid Brass with PVD Finish",
                    MaterialAr = "نحاس صلب مسبوك مع طلاء ذهبي PVD عالي الثبات",
                    Finish = "Diamond Cross-Knurled Texture",
                    FinishAr = "تخريش ماسي مانع للانزلاق وطلاء يدوم للأبد",
                    Color = "Warm Satin Brushed Brass",
                    ColorAr = "ذهبي نحاسي مطفي دافئ",
                    ColorHex = "#D4AF37",
                    Thickness = "160mm Hole Spacing",
                    OriginCountry = "UK Design / Taiwan Production",
                    OriginCountryAr = "تصميم بريطاني / تايوان",
                    ImageUrl = "https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Hardware Pick",
                    BadgeAr = "إكسسوار مميز",
                    IsFeatured = false,
                    IsBestSeller = false,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 45,
                    ShortDescription = "Heavyweight solid brass knurled handles. Provides a secure grip even with wet cooking hands.",
                    ShortDescriptionAr = "مقابض نحاس صلب ثقيلة الوزن مع تخريش ألماسي يسهل الإمساك بها حتى مع الأيدي المبللة أثناء الطهي.",
                    Description = "Elevate every cabinet door and drawer with jewel-like tactile precision. The diamond-knurled barrel provides a satisfying non-slip grip and resists tarnish from culinary acids.",
                    DescriptionAr = "تمنح دواليب مطبخك لمسة فندقية راقية ومريحة في الاستخدام اليومي بفضل متانة النحاس وثبات لونه الذهبي الفاخر."
                },
                new()
                {
                    Id = "prod-12",
                    Name = "Under-Cabinet Slim Aluminum LED Strip (3000K)",
                    NameAr = "مسطرة إضاءة ليد ألومنيوم تحت الدواليب مع سينسور حركة",
                    Price = 320,
                    OriginalPrice = 390,
                    UnitType = "per_linear_meter",
                    SurfaceType = "accessory",
                    CategoryId = "cat-lighting",
                    CategoryName = "Kitchen Lighting",
                    CategoryNameAr = "إضاءة وليد المطبخ",
                    Material = "Anodized Aluminum Profile & Samsung LEDs",
                    MaterialAr = "بروفايل ألومنيوم مبدد للحرارة مع ليدات سامسونج",
                    Finish = "Dotless Opal Diffuser",
                    FinishAr = "مشتت إضاءة حليبي يمنع ظهور نقط الإضاءة المزعجة",
                    Color = "Warm White (3000K) / Natural (4000K)",
                    ColorAr = "إضاءة دافئة مريحة للعين (٣٠٠٠ كلفن)",
                    ColorHex = "#FFF2CC",
                    Thickness = "12x7 mm Ultra Slim",
                    OriginCountry = "Korea / Germany",
                    OriginCountryAr = "كوريا / ألمانيا",
                    ImageUrl = "https://images.unsplash.com/photo-1540518614846-7ede433c4ef2?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Touchless Sensor",
                    BadgeAr = "حساس حركة بدون لمس",
                    IsFeatured = false,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 67,
                    ShortDescription = "High-CRI (95+) seamless under-cabinet task lighting with wave-hand touchless dimmer sensor.",
                    ShortDescriptionAr = "إضاءة ليد خطية فائقة النقاء تبرز جمال أسطح الرخام، تعمل بحركة اليد بالهواء دون لمس المفاتيح بأيدي الطهي.",
                    Description = "High-CRI 95+ LED strips ensure food colors appear true and vibrant on your countertops. Includes a gesture sensor that turns on and dims the light with a gentle wave of your hand.",
                    DescriptionAr = "إضاءة عملية ومريحة تكشف كل تفاصيل التقطيع والطهي على سطح الرخام وتوفر استهلاك الكهرباء بنسبة ٨٥٪."
                },
                new()
                {
                    Id = "prod-13",
                    Name = "Bosch 90cm 5-Burner Built-In Tempered Glass Gas Hob",
                    NameAr = "مسطح غاز بوش ٩٠ سم ٥ شعلة زجاج مقوى مع شعلة ووك مزدوجة",
                    Price = 18500,
                    OriginalPrice = 21000,
                    UnitType = "per_piece",
                    SurfaceType = "accessory",
                    CategoryId = "cat-appliances",
                    CategoryName = "Built-in Appliances",
                    CategoryNameAr = "أجهزة بلت إن",
                    Material = "High-Strength Tempered Glass & Cast Iron Trivets",
                    MaterialAr = "زجاج ألماني سيكوريت أسود مع حوامل زهر صلبة",
                    Finish = "FlameSelect 9-Level Precision Heat",
                    FinishAr = "تحكم دقيق في قوة اللهب بـ ٩ درجات مختلفة",
                    Color = "Black Ceramic Tempered Glass",
                    ColorAr = "أسود ملكي زجاجي عاكس",
                    ColorHex = "#0D0D0D",
                    Thickness = "90 cm Flush/Surface Mount",
                    OriginCountry = "Germany / Spain",
                    OriginCountryAr = "ألمانيا / أسبانيا",
                    ImageUrl = "https://images.unsplash.com/photo-1590794056226-79ef3a8147e1?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Bosch Flagship",
                    BadgeAr = "قمة أجهزة بوش",
                    IsFeatured = true,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 5.0,
                    ReviewsCount = 51,
                    ShortDescription = "Bosch Series 8 gas hob with patented FlameSelect 9 precise flame power levels and dual wok burner.",
                    ShortDescriptionAr = "مسطح بوش الفئة الثامنة مع تكنولوجيا تنظيم اللهب الدقيقة بـ ٩ مستويات، وشعلة ووك سريعة التسوية وأمان كامل ١٠٠٪.",
                    Description = "Equipped with Bosch FlameSelect: adjust the flame power with 9 precisely calibrated levels. Dual-circuit wok burner delivers up to 5.0 kW for searing steaks and stir-fries effortlessly.",
                    DescriptionAr = "الأداء الاحترافي لأشهر الطهاة في منزلك. سهولة مطلقة في تنظيف الزجاج المقوى وأمان أوتوماتيكي يفصل الغاز فور انطفاء الشعلة."
                },
                new()
                {
                    Id = "prod-14",
                    Name = "Kesseböhmer LeMans II Magic Blind Corner Pull-Out Unit",
                    NameAr = "وحدة ركنة سحرية دوارة ألمانية لدواليب المطبخ",
                    Price = 8900,
                    OriginalPrice = 10500,
                    UnitType = "per_set",
                    SurfaceType = "accessory",
                    CategoryId = "cat-accessories",
                    CategoryName = "Storage & Organizers",
                    CategoryNameAr = "منظمات وإكسسوارات",
                    Material = "Anthracite Chrome Steel with Anti-Slip Base",
                    MaterialAr = "شاسيه صلب مطلي أنثراسيت مع أرضيات مضادة للانزلاق",
                    Finish = "Soft-Stopp Hydraulic Self-Closing Action",
                    FinishAr = "حركة هيدروليكية انسيابية تغلق بهدوء تام",
                    Color = "Arena Pure Anthracite & Chrome",
                    ColorAr = "رمادي فحمي مع كروم لامع",
                    ColorHex = "#333333",
                    Thickness = "Fits 900-1000mm Corner Unit",
                    OriginCountry = "Germany",
                    OriginCountryAr = "ألمانيا (Kesseböhmer)",
                    ImageUrl = "https://images.unsplash.com/photo-1556911220-e15b29be8c8f?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Space Optimizer",
                    BadgeAr = "استغلال ذكي للمساحات",
                    IsFeatured = false,
                    IsBestSeller = false,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 18,
                    ShortDescription = "Original German Kesseböhmer LeMans II corner trays with 25kg load capacity per shelf and effortless glide.",
                    ShortDescriptionAr = "الحل الهندسي العبقري لاستغلال ركنة المطبخ الميتة بالكامل. الأرفف تخرج بالكامل أمامك بسلاسة متناهية بحمولة ٢٥ كجم لكل رف.",
                    Description = "Voted the world's most ergonomic kitchen corner solution. The swinging trays glide fully out of the blind cabinet so every pot, pan, and mixer is easily reachable without kneeling or bending.",
                    DescriptionAr = "انسَ المعاناة في الوصول للأواني في عمق الركنة. نظام ألماني أصلي يضاعف المساحة التخزينية لمطبخك ويتحمل الأوزان الثقيلة."
                },
                new()
                {
                    Id = "prod-15",
                    Name = "Silestone Charcoal Soapstone Suede Quartz",
                    NameAr = "سطح كوارتز سايلستون شاركول سويد أسباني مطفي",
                    Price = 2450,
                    OriginalPrice = 2900,
                    UnitType = "per_sqm",
                    SurfaceType = "countertop",
                    CategoryId = "cat-countertops",
                    CategoryName = "Countertops",
                    CategoryNameAr = "أسطح المطبخ (رخام وجرانيت)",
                    Material = "Silestone HybriQ+ Hybrid Mineral Surface",
                    MaterialAr = "سايلستون هايبريك بلس الأسباني الصديق للبيئة",
                    Finish = "Suede Velvet Matte Texture",
                    FinishAr = "ملمس مخملي مطفي فائق الفخامة",
                    Color = "Deep Charcoal with Subtle White Micro-Veins",
                    ColorAr = "رمادي داكن ناعم بعروق صابونية بيضاء دقيقة",
                    ColorHex = "#2E3033",
                    Thickness = "2.0 cm",
                    OriginCountry = "Spain (Cosentino)",
                    OriginCountryAr = "أسبانيا (مجموعة كوزنتينو)",
                    ImageUrl = "https://images.unsplash.com/photo-1600585154526-990dced4db0d?auto=format&fit=crop&q=80&w=1000",
                    Badge = "Eco Luxury",
                    BadgeAr = "صديق للبيئة",
                    IsFeatured = false,
                    IsBestSeller = false,
                    InStock = true,
                    Rating = 4.9,
                    ReviewsCount = 22,
                    ShortDescription = "Cosentino Silestone with HybriQ+ technology. Recycled minerals and 100% renewable energy fabrication.",
                    ShortDescriptionAr = "سطح سايلستون أسباني مستوحى من حجر السوبستون الطبيعي بملمس مخملي ساحر ومقاومة تامة للأحماض والزيوت.",
                    Description = "Manufactured with 99% recycled water and 100% renewable electrical energy. Charcoal Soapstone provides the dramatic rustic mood of natural soapstone with none of the maintenance hassle.",
                    DescriptionAr = "اختيار رائع للمطابخ الحديثة ذات الطابع الصناعي والفندقي. مقاوم للبقع والرطوبة والخدش ولا يحتاج لأي دهانات أو مواد عزل."
                },
                new()
                {
                    Id = "prod-16",
                    Name = "Blum Tandembox Antaro Soft-Close Drawer Box",
                    NameAr = "طقم أدراج مطبخ بلوم أنتارو هيدروليك بلت إن نمساوي",
                    Price = 1450,
                    OriginalPrice = 1750,
                    UnitType = "per_set",
                    SurfaceType = "accessory",
                    CategoryId = "cat-hardware",
                    CategoryName = "Handles & Hardware",
                    CategoryNameAr = "مقابض ومفصلات",
                    Material = "Coated Steel with BLUMOTION Dampers",
                    MaterialAr = "صلب مطلي مع مجاري بلوموشين هيدروليك نمساوية",
                    Finish = "Silk White / Silk Grey Anti-Scratch",
                    FinishAr = "أبيض حريري مع جوانب أنيقة إغلاق ناعم",
                    Color = "Silk White",
                    ColorAr = "أبيض حريري ناصع",
                    ColorHex = "#F7F7F7",
                    Thickness = "500mm Depth (65kg Load)",
                    OriginCountry = "Austria (Blum)",
                    OriginCountryAr = "النمسا (شركة بلوم)",
                    ImageUrl = "https://images.unsplash.com/photo-1556912172-45b7abe8b7e1?auto=format&fit=crop&q=80&w=1000",
                    Badge = "65kg Heavy Duty",
                    BadgeAr = "حمولة شاقة ٦٥ كجم",
                    IsFeatured = false,
                    IsBestSeller = true,
                    InStock = true,
                    Rating = 5.0,
                    ReviewsCount = 59,
                    ShortDescription = "Austrian Blum drawer box with dynamic 65kg load bearing capacity and whisper-quiet soft-close action.",
                    ShortDescriptionAr = "نظام أدراج بلوم النمساوي الأصلي يتحمل أثقل حلل وأواني الطهي حتى ٦٥ كجم ويغلق بهدوء ونعومة دون أي صوت.",
                    Description = "The undisputed world standard in drawer engineering. Features full extension runners for 100% visibility of all contents and feather-light glide even when fully loaded with cast-iron cookware.",
                    DescriptionAr = "يفتح الدرج بالكامل أمامك لرؤية كل محتوياته، مع عمر افتراضي يتجاوز ١٠٠ ألف فتحة وإغلاق معتمد ومضمون مدى الحياة."
                }
            };
    }
}
