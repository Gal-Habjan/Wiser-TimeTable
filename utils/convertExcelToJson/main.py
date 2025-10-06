import pandas as pd
import json
import hashlib
filePath = "urnik.xls"

def rename_keys(data):
    for item in data:
        for key, value in item.items():
            if value == "RIT 2 UN - UP2 RV 3, RIT 3 UN RV 1":
                item[key] = "RIT 2 UN - UP2 RV 3"
    return data
def rename_key(data, old_key, new_key):
    for item in data:
        if old_key in item:
            item[new_key] = item.pop(old_key)
    return data
if __name__ == '__main__':
    df = pd.read_excel(filePath,skiprows=2,usecols=lambda x: 'Unnamed' not in x)
    json_data = df.to_json(orient='records')
    data = json.loads(json_data)

    # Rename specific keys if found
    # data = rename_keys(data)

    data = rename_key(data, "Izvajanje", "Opis")
    # Wrap the JSON data with a "Classes" key
    json_data_wrapped = {

        "Classes": data,
    }


    output_json_file = 'school.json'

    # Write the JSON data to the output file
    with open(output_json_file, 'w') as outfile:
        json.dump(json_data_wrapped, outfile)