<?xml version="1.0" encoding="UTF-8"?>
<tileset name="MineTileset" tilewidth="32" tileheight="32" tilecount="256" columns="16">
 <image source="MineTileset.png" width="512" height="512"/>
 <terraintypes>
  <terrain name="Ground" tile="-1"/>
  <terrain name="Pit" tile="-1"/>
  <terrain name="Cliff" tile="-1"/>
 </terraintypes>
 <tile id="0" terrain="1,1,1,0">
  <objectgroup draworder="index">
   <object id="2" type="L1: ProjectilePassible" x="15.375" y="32.5">
    <polygon points="0,0 5.75,-12.375 17.25,-16.875 17,-33.75 -16,-33.125 -15.75,-0.375"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="1" terrain="1,1,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-1.125" y="15.5">
    <polygon points="0,1 33.625,0.5 33.625,-17.125 0.125,-16.5"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="2" terrain="1,1,0,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-0.875" y="15.375">
    <polygon points="0,0 12.875,4 16.875,17.5 33.5,17.125 33.375,-16.125 0,-16.25"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="3" terrain="0,0,0,0"/>
 <tile id="16" terrain="1,0,1,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-0.25" y="-0.875">
    <polygon points="-0.625,-0.125 -0.125,33.125 16.25,33.75 16.25,-0.375"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="17" terrain="0,0,0,0"/>
 <tile id="18" terrain="0,1,0,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="15.5" y="-1.5">
    <polygon points="0,0 0.125,34.25 17.375,34 17,0.625"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="19" terrain="0,0,0,0"/>
 <tile id="32" terrain="1,0,1,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-0.625" y="-0.75">
    <polygon points="0,0 -0.125,32.875 33.125,33.25 32.875,17 20.5,11.25 15.625,0.125"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="33" terrain="0,0,1,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-1.75" y="15.625">
    <polygon points="0,0 34.5,0.625 34.375,17.5 1,16.75"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="34" terrain="0,1,1,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="16" y="-0.875">
    <polygon points="0,0 16.625,0.375 16.25,33.625 -16.625,33.25 -16.5,16.875 -3,13.375"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="35" terrain="0,0,0,0" probability="0"/>
 <tile id="36" terrain="0,0,0,0" probability="0"/>
 <tile id="48" terrain="0,0,0,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="16" y="32.5">
    <polygon points="0,0 6.375,-10.375 16.75,-16.375 16.5,-0.25"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="49" terrain="0,0,1,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-0.75" y="16.875">
    <polygon points="0,0 33.125,0.375 33.125,15.875 0.25,15.5"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="50" terrain="0,0,1,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-1" y="17.375">
    <polygon points="0,0 11,2.75 17.5,15.375 0.375,15.625"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="64" terrain="0,1,0,1">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="15.375" y="-2">
    <polygon points="0,0 0.125,35.125 17.25,34.375 17.125,1.375"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="65" terrain="1,1,1,1"/>
 <tile id="66" terrain="1,0,1,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-0.375" y="-1">
    <polygon points="0,0 15.875,0.25 16,34.125 -0.25,33.5"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="80" terrain="0,1,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="15.5" y="-1.125">
    <polygon points="0,0 16.875,0.375 17,19 5.375,13.125"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="81" terrain="1,1,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-0.625" y="-0.625">
    <polygon points="0,0 33,0.125 33.25,18.375 -0.125,18"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="82" terrain="1,0,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: ProjectilePassible" x="-1" y="-0.875">
    <polygon points="0,0 17.5,0.125 12.875,13 0.25,18.25"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="96" terrain="2,2,2,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.75" y="-0.875">
    <polygon points="0,0 33.125,0.625 33,33.5 -0.5,33.5"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="97" terrain="2,2,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-1">
    <polygon points="0,0 33.625,0.25 33.375,34.375 0.125,33.875"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="98" terrain="2,2,0,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.625" y="-0.375">
    <polygon points="0,0 32.875,-0.125 33,33.125 0.25,33"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="112" terrain="2,0,2,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.875" y="-1.125">
    <polygon points="0,0 33.25,0.625 33.5,34.125 0.25,33.625"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="113" terrain="0,0,0,0"/>
 <tile id="114" terrain="0,2,0,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.875" y="-0.875">
    <polygon points="0,0 33.25,0.25 33.375,34.375 0,33.375"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="128" terrain="2,0,2,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.75" y="-0.625">
    <polygon points="0,0 32.875,0 33.125,33.625 0.125,33.25"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="129" terrain="0,0,2,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1.125" y="-1">
    <polygon points="0,0 33.5,0.125 33.625,33.875 0.375,33.375"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="130" terrain="0,2,2,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-0.75">
    <polygon points="0,0 33.375,0.125 33.25,33.875 0.25,33.25"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="144" terrain="0,0,0,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="2" y="32.25">
    <polygon points="0,0 9.75,-22.125 30.75,-30.75 30.25,0.5"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="145" terrain="0,0,2,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-0.875">
    <polygon points="0,0 33.75,0.125 33.5,34.125 0,33.625"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="146" terrain="0,0,2,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.5" y="1.25">
    <polygon points="0,0 20.125,6.875 31.125,31.25 -0.625,31.875"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="160" terrain="0,2,0,2">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-1.125">
    <polygon points="0,0 33.5,0.25 33.5,34.125 0.125,33.75"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="161" terrain="2,2,2,2"/>
 <tile id="162" terrain="2,0,2,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1.125" y="-1">
    <polygon points="0,0 33.75,0.375 33.5,34 0.5,33.75"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="176" terrain="0,2,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="1.375" y="-1">
    <polygon points="0,0 30.875,0.25 31.5,31.25 8.75,25.5"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="177" terrain="2,2,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1.25" y="-0.625">
    <polygon points="0,0 33.875,0.125 33.75,33.875 0.375,33.625"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="178" terrain="2,0,0,0">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-1.25">
    <polygon points="0,0 31.875,0.5 20.875,22.625 0.25,31.375"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="180">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1.125" y="-1.125">
    <polygon points="0,0 33.375,0.5 33.75,34.5 0,34.5"/>
   </object>
  </objectgroup>
  <animation>
   <frame tileid="180" duration="100"/>
   <frame tileid="197" duration="100"/>
   <frame tileid="181" duration="100"/>
   <frame tileid="180" duration="100"/>
   <frame tileid="198" duration="100"/>
   <frame tileid="181" duration="100"/>
   <frame tileid="182" duration="100"/>
   <frame tileid="196" duration="100"/>
   <frame tileid="180" duration="100"/>
   <frame tileid="198" duration="100"/>
   <frame tileid="181" duration="100"/>
   <frame tileid="197" duration="100"/>
  </animation>
 </tile>
 <tile id="181">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.875" y="-1.125">
    <polygon points="0,0 33.5,0.625 33.625,34.375 -0.25,34.125"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="182">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-1.125">
    <polygon points="0,0 33.5,0.375 33.375,34.125 0.25,33.625"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="196">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-0.875">
    <polygon points="0,0 33.25,0.375 33.5,34 0.375,33.75"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="197">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-1" y="-1.25">
    <polygon points="0,0 33.125,0.5 33.375,34.375 0,34"/>
   </object>
  </objectgroup>
 </tile>
 <tile id="198">
  <objectgroup draworder="index">
   <object id="1" type="L1: Barrier" x="-0.75" y="-0.875">
    <polygon points="0,0 33.125,0.25 33.25,34.375 -0.125,33.875"/>
   </object>
  </objectgroup>
 </tile>
</tileset>
