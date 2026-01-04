=== Testing ===

#nm_none
"Uhhhhhhh... FMCL." #chl_clown+pop #nm_Chud #d_chud
"Don't say that, you look fine." #chr_girl+bounce #nm_Girl #d_girl
"Don't talk to this guy babe, he's a chud." #chr_dude+bounce #nm_Dude #chr_girl_eyes_lowered+shakevertical #d_dude
"AHHHH" #chl_clown_freaky+shake #chl_dude #nm_Chud #chr_girl_eyes_closed+shake #d_chud

... #chr_clown_clear #chl_dude #nm_Chud
End #nm_none
-> END

=== Testing_Animations ===

#nm_none

Spawning Character #chc_clown

Testing Standard Shake. #an_clown_shake
Testing Inline Syntax (Sprite Change + Shake). #chc_clown_freaky+shake

Testing Hop with Parameters (Height 20, Duration 0.5s). #an_clown_hop_20_0.5
Testing Reverse Hop. #an_clown_reversehop

Testing Bounce on Dude. #chr_dude+bounce
Testing Flash on Clown. #chc_clown+flash
Testing Dodge on Clown. #chc_clown+dodge
Testing Pop on Clown. #chc_clown+pop
Testing Vertical Shake on Dude. #chr_dude+shakevertical

Clearing Characters. #chl_clown_clear #chr_dude_clear
Tests Complete.
-> END

=== Testing_Voices ===

#nm_none
Testing dialogue voice system.

#chc_clown #nm_Chud
"Hello! This line should play Chud's voice." #d_chud
"Another line from Chud with voice." #d_chud

#chr_girl #nm_Girl
"Hi there! This is the Girl speaking." #d_girl

#nm_none
This is narration. No voice should play here.

#nm_Chud
"Back to Chud again!" #d_chud

#nm_none
Voice test complete.
-> END

=== Testing_TextAnimator ===

#nm_none
Testing Text Animator Effects.

Basic Wiggle: <wiggle>Wiggle Wiggle</wiggle>
High Intensity Wiggle: <wiggle a=2>INTENSE WIGGLE</wiggle>
Low Frequency Wiggle: <wiggle f=0.5>Slow Wiggle</wiggle>

Basic Shake: <shake>Shaking Text</shake>
Violent Shake: <shake a=3>VIOLENT SHAKE</shake>

Wave Effect: <wave>Wavy Text goes up and down</wave>
Fast Wave: <wave f=5>Super Fast Wave</wave>

Rotation ("Rot"): <rot>Rotating Text</rot>
Swing: <swing>Swinging Text</swing>

Rainbow: <rainb>Taste the Rainbow</rainb>
Rainbow (Fast): <rainb f=2>Fast Rainbow</rainb>

Fade In: <fade>Fading In Text</fade>
Delayed Words: Wait for it... <waitfor=1> Now!

<wiggle><rainb>Wiggling Rainbow</rainb></wiggle>
<shake a=0.5><wave f=2>Shaking Wave</wave></shake>

Typewritte Speed: Start normal. <speed=0.1>Slooooooooooow... <speed=10>Faaaaaaaaaaaaaaaast!

Test Complete.
-> END