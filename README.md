# DoorInterfaceControl

# This application I built to interface with our Isoans Pure Access Manager Door system.  
# The Isonas software is on a server local to our network.
# Isonas has documentation on its API calls.  This application follows their V1 API.  
# Isonas also has Pure Access Cloud system which is hosted and uses a V2 API.
# This application pulls in all the doors in the system displays the MAC Address, Reader type, 
# Door State (e.g. Propped Open) , and Door Unlock State (Locked or Unlocked).
# I added buttons to unlock all the doors in case of a fire, also added a button to return
# the system to normal.  Then you can check individual doors (1 or more) and change the status.
# The status changes are Admit which sets the door to unlock just like a keyfob read then lock again.
# Lockdown which sets the door to a lockdown status and only master keyfobs can unlock the door.
# Schedule which returns the door to is normal state based on the schedule set in the system.
# Unlock which permanently unlocks the door until another state is applied to the door or midnight 
# when a schedule is published
# I have locked it down to AD groups that match either IT or HR.  That way only people that control the 
# Door system can use this application.
# This door is build on MVVM model using Prism (https://github.com/PrismLibrary/Prism).  
# It will require Prism to be added to your programming environment.
#
# Feel free to take the code and modify it to fit your needs.  All I ask is give me credit and point people
# to my github location.
