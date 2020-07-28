# LTEReboot
This is a Script that reboots your O² HomeSpot LTE Router once it has lost connection. It seems to have a problem with connecting back once there has been an interruption, this should fix the problem by checking for a connection and rebooting the HomeSpot if so.


<div style="background-color: black; color: #ffffff">
<h1> How To Use </h1>
<ul>
  <li> Open CMD </li>
  <li> Navigate to the directory containing <b>LTEReboot.exe</b> using <em>cd C:\Program Files\etc</em> - Note that this is an example path and you should use the one your exe is at</li>
  <li> Once youre in the directory type: <em>LTEReboot [YourRouterControlPanelURL] [YourRouterControlPanelPassword] [ProbeIP]</em> without the "[" and "]" (ProbeIP is used to check if youre able to connect to that IP and therefore the internet. I use 8.8.8.8 which is Googles DNS Server and good for measurements like this) </li> 
  <li> If everything was done correctly you should be done and your O² LTE Router should be rebooting (if it is unable to connect to the ProbeIP) </li>
</ul>
</div>
