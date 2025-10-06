import json
import hashlib
def hash_data(data):
    # Convert the data to a JSON string and hash it using SHA256
    json_string = json.dumps(data, sort_keys=True)
    return hashlib.sha256(json_string.encode()).hexdigest()
# Load data from school.json
with open('school.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

# List of substrings to check in Skupina
skupine_to_remove = []
classes_to_remove = ["UMETNA INTELIGENCA", "ANGLEŠČINA - JEZIK STROKE", "GEOGRAFSKI INFORMACIJSKI SISTEMI", "PODJETNIŠTVO", "SIGNALI IN SLIKE",]

# Filter out entries where Skupina contains any unwanted substrings
filtered_classes = [
    entry for entry in data["Classes"]
    if entry["Skupina"] == "RIT 3 UN UP1, RIT 3 UN UP2" or (
        not any(skupina in entry["Skupina"] for skupina in skupine_to_remove) and
        not any(opis in entry["Opis"] for opis in classes_to_remove)
    )
]

# Recalculate hash for the filtered data
new_hash = hash_data(filtered_classes)

# Create a new dictionary with filtered classes and updated hash
filtered_data = {
    "Hash": new_hash,
    "Classes": filtered_classes
}
# Save the filtered data to schol.json
with open('schoolFiltered.json', 'w', encoding='utf-8') as f:
    json.dump(filtered_data, f, ensure_ascii=False, indent=4)

print("Filtered data saved to schol.json.")