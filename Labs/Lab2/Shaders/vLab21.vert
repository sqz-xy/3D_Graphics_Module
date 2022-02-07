#version 330

in vec3 vColour;
in vec3 vPosition;

out vec4 oColour;

void main()
{
	gl_Position = vec4(vPosition, 1);
	oColour = vec4(vColour, 1);
}
