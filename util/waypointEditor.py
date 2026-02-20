import os, json

def load(filename):
    if not os.path.exists(filename):
        return []
    with open (filename, 'r') as f:
        return json.load(f)["Waypoints"]
    
def save(filename, data):
    with open(filename, "w") as f:
        json.dump({"Waypoints": data}, f, indent=4)

def list_waypoints(data):
    n = 0
    print("Select a Waypoint")
    for i in data:
        print(f'{n} - {i["name"]}')
        n += 1
    print(f'{n+1} - Create New Waypoint')

def prompt(d):
    list_waypoints


def main():
    d = load('Assets/Resources/Waypoints.json')
    list_waypoints(d)
    
if __name__ == "__main__":
    main()
