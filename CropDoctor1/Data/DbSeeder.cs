using CropDoctor1.Data;
using CropDoctor1.Models;

public static class DbSeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (context == null)
            {
                Console.WriteLine("Error: Could not get ApplicationDbContext for seeding.");
                return;
            }

            context.Database.EnsureCreated();

            // --- Seed Disease Information ---
            if (!context.DiseaseInfos.Any())
            {
                Console.WriteLine("--> Seeding DiseaseInfos Data...");
                var diseases = new List<DiseaseInfo>
                {
                    // --- THIS IS THE COMPLETE LIST ---
                    new DiseaseInfo { DiseaseKey = "Apple___Apple_scab", PlantName = "Apple", DiseaseName = "Apple Scab", Cause = "Caused by the fungus Venturia inaequalis.", RecommendedSolution = "Prune trees to improve air circulation. Apply fungicides." },
                    new DiseaseInfo { DiseaseKey = "Apple___Black_rot", PlantName = "Apple", DiseaseName = "Black Rot", Cause = "Caused by the fungus Botryosphaeria obtusa.", RecommendedSolution = "Prune out dead or cankered branches." },
                    new DiseaseInfo { DiseaseKey = "Apple___Cedar_apple_rust", PlantName = "Apple", DiseaseName = "Cedar Apple Rust", Cause = "Fungus Gymnosporangium juniperi-virginianae.", RecommendedSolution = "Remove nearby cedar trees or apply fungicides." },
                    new DiseaseInfo { DiseaseKey = "Apple___healthy", PlantName = "Apple", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Maintain good watering and fertilization." },
                    new DiseaseInfo { DiseaseKey = "Blueberry___healthy", PlantName = "Blueberry", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Ensure soil remains acidic and well-drained." },
                    new DiseaseInfo { DiseaseKey = "Cherry___Powdery_mildew", PlantName = "Cherry", DiseaseName = "Powdery Mildew", Cause = "Caused by the fungus Podosphaera clandestina.", RecommendedSolution = "Improve air circulation and apply fungicides." },
                    new DiseaseInfo { DiseaseKey = "Cherry___healthy", PlantName = "Cherry", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "The plant appears healthy. Monitor for pests." },
                    new DiseaseInfo { DiseaseKey = "Corn___Cercospora_leaf_spot Gray_leaf_spot", PlantName = "Corn (Maize)", DiseaseName = "Gray Leaf Spot", Cause = "Fungus Cercospora zeae-maydis.", RecommendedSolution = "Use resistant hybrids and practice crop rotation." },
                    new DiseaseInfo { DiseaseKey = "Corn___Common_rust", PlantName = "Corn (Maize)", DiseaseName = "Common Rust", Cause = "Fungus Puccinia sorghi.", RecommendedSolution = "Most hybrids have good resistance. Fungicides can be used in severe cases." },
                    new DiseaseInfo {  DiseaseKey = "Corn___Northern_Leaf_Blight", PlantName = "Corn (Maize)", DiseaseName = "Northern Leaf Blight", Cause = "Fungus Exserohilum turcicum.", RecommendedSolution = "Plant resistant hybrids and use tillage." },
                    new DiseaseInfo {  DiseaseKey = "Corn___healthy", PlantName = "Corn (Maize)", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Ensure proper nutrient and water management." },
                    new DiseaseInfo {  DiseaseKey = "Grape___Black_rot", PlantName = "Grape", DiseaseName = "Black Rot", Cause = "Fungus Guignardia bidwellii.", RecommendedSolution = "Remove infected parts and apply fungicides." },
                    new DiseaseInfo {  DiseaseKey = "Grape___Esca_(Black_Measles)", PlantName = "Grape", DiseaseName = "Esca (Black Measles)", Cause = "A complex trunk disease caused by fungi.", RecommendedSolution = "No cure. Prune well below symptoms and sanitize tools." },
                    new DiseaseInfo {  DiseaseKey = "Grape___Leaf_blight_(Isariopsis_Leaf_Spot)", PlantName = "Grape", DiseaseName = "Leaf Blight", Cause = "Fungus Pseudocercospora vitis.", RecommendedSolution = "Generally a minor disease. Good air circulation helps." },
                    new DiseaseInfo {  DiseaseKey = "Grape___healthy", PlantName = "Grape", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Maintain a good pruning and spray schedule." },
                    new DiseaseInfo {  DiseaseKey = "Orange___Haunglongbing_(Citrus_greening)", PlantName = "Orange", DiseaseName = "Citrus Greening", Cause = "Bacterium spread by the Asian Citrus Psyllid.", RecommendedSolution = "No cure. Control psyllid populations and remove infected trees." },
                    new DiseaseInfo {  DiseaseKey = "Peach___Bacterial_spot", PlantName = "Peach", DiseaseName = "Bacterial Spot", Cause = "Bacterium Xanthomonas campestris.", RecommendedSolution = "Plant resistant varieties. Apply copper-based bactericides." },
                    new DiseaseInfo {  DiseaseKey = "Peach___healthy", PlantName = "Peach", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Continue with proper thinning and watering." },
                    new DiseaseInfo {  DiseaseKey = "Pepper,_bell___Bacterial_spot", PlantName = "Bell Pepper", DiseaseName = "Bacterial Spot", Cause = "Xanthomonas bacteria. Spread by water splash.", RecommendedSolution = "Use disease-free seeds. Rotate crops. Apply copper-based bactericides." },
                    new DiseaseInfo {  DiseaseKey = "Pepper,_bell___healthy", PlantName = "Bell Pepper", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Provide consistent moisture and support." },
                    new DiseaseInfo {  DiseaseKey = "Potato___Early_blight", PlantName = "Potato", DiseaseName = "Early Blight", Cause = "Fungus Alternaria solani.", RecommendedSolution = "Rotate crops and apply protective fungicides." },
                    new DiseaseInfo {  DiseaseKey = "Potato___Late_blight", PlantName = "Potato", DiseaseName = "Late Blight", Cause = "Oomycete Phytophthora infestans.", RecommendedSolution = "Destroy infected plants. Use preventative fungicides." },
                    new DiseaseInfo {  DiseaseKey = "Potato___healthy", PlantName = "Potato", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Continue to monitor for pests and water consistently." },
                    new DiseaseInfo {  DiseaseKey = "Raspberry___healthy", PlantName = "Raspberry", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Ensure good pruning for air circulation." },
                    new DiseaseInfo {  DiseaseKey = "Soybean___healthy", PlantName = "Soybean", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Monitor for common pests like aphids." },
                    new DiseaseInfo {  DiseaseKey = "Squash___Powdery_mildew", PlantName = "Squash", DiseaseName = "Powdery Mildew", Cause = "Several species of fungi.", RecommendedSolution = "Improve air circulation. Apply horticultural oil or neem oil." },
                    new DiseaseInfo {  DiseaseKey = "Strawberry___Leaf_scorch", PlantName = "Strawberry", DiseaseName = "Leaf Scorch", Cause = "Fungus Diplocarpon earlianum.", RecommendedSolution = "Remove infected leaves. Use protective fungicides." },
                    new DiseaseInfo {  DiseaseKey = "Strawberry___healthy", PlantName = "Strawberry", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "Use straw mulch to keep berries clean." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Bacterial_spot", PlantName = "Tomato", DiseaseName = "Bacterial Spot", Cause = "Xanthomonas bacteria. Spreads in warm, rainy weather.", RecommendedSolution = "Avoid overhead watering. Apply copper-based bactericides." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Early_blight", PlantName = "Tomato", DiseaseName = "Early Blight", Cause = "Fungus Alternaria solani.", RecommendedSolution = "Prune lower leaves. Apply fungicides like chlorothalonil." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Late_blight", PlantName = "Tomato", DiseaseName = "Late Blight", Cause = "Oomycete Phytophthora infestans.", RecommendedSolution = "Act fast. Remove infected plants. Use preventative fungicides." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Leaf_Mold", PlantName = "Tomato", DiseaseName = "Leaf Mold", Cause = "Fungus Passalora fulva.", RecommendedSolution = "Improve air circulation and reduce humidity." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Septoria_leaf_spot", PlantName = "Tomato", DiseaseName = "Septoria Leaf Spot", Cause = "Fungus Septoria lycopersici.", RecommendedSolution = "Remove infected leaves. Mulch well. Apply fungicides." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Spider_mites Two-spotted_spider_mite", PlantName = "Tomato", DiseaseName = "Spider Mites (Two-spotted)", Cause = "Arachnid Tetranychus urticae.", RecommendedSolution = "Spray plants with water. Use insecticidal soap or neem oil." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Target_Spot", PlantName = "Tomato", DiseaseName = "Target Spot", Cause = "Fungus Corynespora cassiicola.", RecommendedSolution = "Improve air circulation. Rotate crops. Apply fungicides." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Tomato_Yellow_Leaf_Curl_Virus", PlantName = "Tomato", DiseaseName = "Tomato Yellow Leaf Curl Virus", Cause = "Virus (TYLCV) transmitted by whiteflies.", RecommendedSolution = "No cure. Control whitefly populations and remove infected plants." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___Tomato_mosaic_virus", PlantName = "Tomato", DiseaseName = "Tomato Mosaic Virus", Cause = "Virus (ToMV) that is mechanically transmitted.", RecommendedSolution = "No cure. Remove infected plants. Wash hands and sanitize tools." },
                    new DiseaseInfo {  DiseaseKey = "Tomato___healthy", PlantName = "Tomato", DiseaseName = "Healthy", Cause = "N/A", RecommendedSolution = "The plant appears healthy. Continue monitoring." },
                    new DiseaseInfo {  DiseaseKey = "Background_without_leaves", PlantName = "N/A", DiseaseName = "No Leaf Detected", Cause = "Image does not contain a recognizable leaf.", RecommendedSolution = "Please upload a clear photo of a single plant leaf against a simple background." }
                };
                context.DiseaseInfos.AddRange(diseases);
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> DiseaseInfos data already exists. Skipping seed.");
            }

            // --- Seed Crop Information ---
            if (!context.CropInfos.Any())
            {
                Console.WriteLine("--> Seeding CropInfos Data...");
                var crops = new List<CropInfo>
                {
                    new CropInfo {
                         Name = "Tomato", TemperatureRange = "21-29°C", MarketInsight = "Consistently high demand.", SowingGuide = "Sow seeds indoors 6-8 weeks before last frost.", PlantingSeasons = new List<string> { "Spring", "Summer" }, SuitableSoilTypes = new List<string> { "Loamy", "Well-Drained" }
                    },
                    new CropInfo {
                        Name = "Potato", TemperatureRange = "15-20°C", MarketInsight = "A staple crop with stable demand.", SowingGuide = "Plant seed potatoes 10-15 cm deep.", PlantingSeasons = new List<string> { "Spring" }, SuitableSoilTypes = new List<string> { "Sandy", "Loamy", "Acidic" }
                    },
                    new CropInfo {
                        Name = "Corn (Maize)", TemperatureRange = "25-33°C", MarketInsight = "High demand for human consumption and animal feed.", SowingGuide = "Sow directly into the garden after the last frost.", PlantingSeasons = new List<string> { "Summer" }, SuitableSoilTypes = new List<string> { "Loamy", "Silty" }
                    },
                    new CropInfo {
                         Name = "Lettuce", TemperatureRange = "15-18°C", MarketInsight = "Quick turnaround crop. High demand for fresh, local salad greens.", SowingGuide = "Sow seeds shallowly in cool soil.", PlantingSeasons = new List<string> { "Spring", "Autumn" }, SuitableSoilTypes = new List<string> { "Silty", "Well-Drained", "Rich in Organic Matter" }
                    },
                    new CropInfo {
                         Name = "Wheat", TemperatureRange = "12-25°C", MarketInsight = "A major global commodity. Best for large-scale farming.", SowingGuide = "Winter wheat is sown in the autumn.", PlantingSeasons = new List<string> { "Autumn", "Winter" }, SuitableSoilTypes = new List<string> { "Clay", "Loamy" }
                    }
                };
                context.CropInfos.AddRange(crops);
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> CropInfos data already exists. Skipping seed.");
            }
        }
    }
}