#version 400

uniform mat4 modelViewProjectionMatrix;

in vec3 inVertexPosition;
in vec2 inTextureCoordinates;

out gl_PerVertex // Required for Program Pipelines
{
	vec4 gl_Position;
	float gl_PointSize;
	float gl_ClipDistance[];
};
out vec2 passTextureCoordinates;

void main(void)
{
	gl_Position = modelViewProjectionMatrix * inVertexPosition;
	passTextureCoordinates = inTextureCoordinates;
}