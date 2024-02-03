using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwentyFiftyMaterialsCore.Models
{
    public class TFParameter<T> : IEquatable<TFParameter<T>>, ICloneable
    {
        [JsonProperty("Value")]
        public T Value { get; set; }

        [JsonProperty("ProjectName")]
        public string Name { get; set; }

        [JsonProperty("Visible")]
        public bool Visible { get; set; }


        public TFParameter(T value)
        {
            this.Value = value;
        }

        public TFParameter()
        {

        }

        public override bool Equals(object obj) => this.Equals(obj as TFParameter<T>);
        public bool Equals(TFParameter<T> other)
        {
            if (other is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return Value.Equals(other.Value);
        }
        //public override int GetHashCode() => (Value).GetHashCode();

        public static bool operator ==(TFParameter<T> lhs, TFParameter<T> rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }
        public static bool operator !=(TFParameter<T> lhs, TFParameter<T> rhs) => !(lhs == rhs);

        public TFParameter<T> DeepClone()
        {
            TFParameter<T> target = (TFParameter<T>)this.Clone();
            var json = JsonConvert.SerializeObject(target);
            TFParameter<T> other = JsonConvert.DeserializeObject<TFParameter<T>>(json);
            return other;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
