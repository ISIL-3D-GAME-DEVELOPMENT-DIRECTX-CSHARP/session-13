using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;

using Device = SharpDX.Direct3D11.Device;
using D3DBuffer = SharpDX.Direct3D11.Buffer;
using D3DMapFlags = SharpDX.Direct3D11.MapFlags;
using System.Runtime.InteropServices;
using SharpDX.Direct3D;
using Sesion2_Lab01.com.isil.content;

namespace Sesion2_Lab01.com.isil.shader.d3d {
    public class Shader3DProgram {

        public float mRotationX;
        public float mRotationY;
        public float mRotationZ;

        private Device mDevice;
        private DeviceContext mDeviceContext;

        private VertexShader mVertexShader;
        private PixelShader mPixelShader;
        private InputLayout mInputLayout;

        private D3DBuffer mMiscInputBuffer;
        private BufferDescription mMiscInputBufferDescription;

        private Matrix mWorld;
        private Matrix mRotationXMatrix;
        private Matrix mRotationYMatrix;
        private Matrix mRotationZMatrix;

        public VertexShader VertexShader    { get { return mVertexShader; } }
        public PixelShader PixelShader      { get { return mPixelShader; } }
        public InputLayout InputLayout      { get { return mInputLayout; } }

        public float X {
            get { return mWorld.M41; }
            set { mWorld.M41 = value; }
        }

        public float Y {
            get { return mWorld.M42; }
            set { mWorld.M42 = value; }
        }

        public float ScaleX {
            get { return mWorld.M11; }
            set { mWorld.M11 = value; }
        }

        public float ScaleY {
            get { return mWorld.M22; }
            set { mWorld.M22 = value; }
        }

        public float RotationX {
            get { return mRotationX; }
            set {
                if (mRotationX != value) {
                    mRotationX = value;

                    mRotationXMatrix = Matrix.RotationX(mRotationX);
                }
            }
        }

        public float RotationY {
            get { return mRotationY; }
            set {
                if (mRotationY != value) {
                    mRotationY = value;

                    mRotationYMatrix = Matrix.RotationY(mRotationY);
                }
            }
        }

        public float RotationZ {
            get { return mRotationZ; }
            set {
                if (mRotationZ != value) {
                    mRotationZ = value;

                    mRotationZMatrix = Matrix.RotationZ(mRotationZ);
                }
            }
        }

        public Shader3DProgram(Device device) {
            mDevice = device;
            mDeviceContext = device.ImmediateContext;

            mWorld = mRotationXMatrix = mRotationYMatrix = mRotationZMatrix = Matrix.Identity;
        }

        public void Load(string path) {
            CompilationResult vertexShaderByteCode = ShaderBytecode.CompileFromFile(path,
                "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            mVertexShader = new VertexShader(mDevice, vertexShaderByteCode);

            // compilamos nuestro fragment shader
            CompilationResult pixelShaderByteCode = ShaderBytecode.CompileFromFile(path,
                "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            mPixelShader = new PixelShader(mDevice, pixelShaderByteCode);

            ShaderSignature shaderSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);

            // creamos el contenedor de nuestras variables de entrada
            InputElement[] inputElements = new InputElement[4];
            // segun nuestro shader es: Posicion de los vertices
            inputElements[0] = new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0);
            // segun nuestro shader es: El color de cada vertice
            inputElements[1] = new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0);
            // segun nuestro shader es: El color de cada vertice
            inputElements[2] = new InputElement("NORMAL", 0, Format.R32G32B32_Float, 28, 0);
            // segun nuestro shader es: El color de cada vertice
            inputElements[3] = new InputElement("TEXTCOORD", 0, Format.R32G32_Float, 40, 0);

            // construimos el Input Layout, le pasamos el Shader Signature que contiene la referencia
            // de nuestro Shader, el Input Element que contiene nuestro parametros de entrada, y 
            // mediante esto crea un Objeto para poder enviarlo a la tarjeta de video para poder usarlo
            mInputLayout = new InputLayout(mDevice, shaderSignature, inputElements);

            // ahora creamos una Descripcion para almacenar la informacion de nuestro nuevo bloque
            // de parametros en nuestro Shader
            mMiscInputBufferDescription.Usage = ResourceUsage.Dynamic;
            mMiscInputBufferDescription.SizeInBytes = Utilities.SizeOf<Shader3DInputParameters>();
            mMiscInputBufferDescription.BindFlags = BindFlags.ConstantBuffer;
            mMiscInputBufferDescription.CpuAccessFlags = CpuAccessFlags.Write;
            mMiscInputBufferDescription.OptionFlags = 0;
            mMiscInputBufferDescription.StructureByteStride = 0;

            // creamos un Buffer para almacenar nuestro parametros sueltos
            mMiscInputBuffer = new D3DBuffer(mDevice, mMiscInputBufferDescription);
        }

        public void SetConstantInputParameter<T>(D3DBuffer inputBuffer,
            int subResource, int slot, ref T parametersStruct) {

            // Retrieve the Data Stream from the Shader, so now can we access the Memory Pointer
            // to modify the Input Parameters from the Shader
            DataStream outData = null;
            mDevice.ImmediateContext.MapSubresource(inputBuffer,
                MapMode.WriteDiscard, D3DMapFlags.None, out outData);

            // Now that we set the new values, let's say to the Memory Pointer that we
            // changed the values and update the Memory Pointer
            Marshal.StructureToPtr(parametersStruct, outData.DataPointer, true);

            // Now we re-enable the Buffer
            mDevice.ImmediateContext.UnmapSubresource(inputBuffer, subResource);
            // Now send the Buffer to the Vertex Shader
            mDevice.ImmediateContext.VertexShader.SetConstantBuffer(slot, inputBuffer);
        }

        public void Update(D3DBuffer indexBuffer, VertexBufferBinding vertexBuffer) {
            mDeviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);
            mDeviceContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
        }

        public void Draw(int indicesCount, Matrix transformation, NTexture2D texture, PrimitiveTopology topology) {
            transformation = mRotationXMatrix * mRotationYMatrix * mRotationZMatrix * mWorld * transformation;
            transformation.Transpose();

            Shader3DInputParameters inputParameters = Shader3DInputParameters.EMPTY;
            inputParameters.transformation = transformation;
            inputParameters.ambientColor = Vector4.One;
            inputParameters.diffuseLighting = Vector4.One;
            inputParameters.ambientIntensity = 1.1f;
            inputParameters.lightDirection = Vector3.Zero;

            // Set the Shader Constant Parameters
            SetConstantInputParameter<Shader3DInputParameters>(mMiscInputBuffer,
                0, 0, ref inputParameters);

            mDeviceContext.InputAssembler.InputLayout = mInputLayout;
            // definimos ahora de que manera se va a tratar la data que entra y como se dibuja
            mDeviceContext.InputAssembler.PrimitiveTopology = topology;

            // aqui vamos a transferir el Vertex y Fragment Shader que ya habiamos creado
            mDeviceContext.VertexShader.Set(mVertexShader);
            mDeviceContext.PixelShader.Set(mPixelShader);

            // ahora seteamos nuestra textura en nuestro Shader
            if (texture != null) {
                mDeviceContext.PixelShader.SetShaderResource(0, texture.TextureResource);
                mDeviceContext.PixelShader.SetSampler(0, texture.SamplerState);
            }

            // dibujamos nuestros vertices!
            mDeviceContext.DrawIndexed(indicesCount, 0, 0);
        }
    }
}
